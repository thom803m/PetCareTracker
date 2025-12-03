using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;
using System.Security.Claims;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly IPetRepository _petRepo;

        public PetsController(IPetRepository petRepo)
        {
            _petRepo = petRepo;
        }

        [HttpGet] // åbent
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetPets()
        {
            var pets = await _petRepo.GetAllAsync();
            return Ok(pets.Select(PetToDTO).ToList());
        }

        [HttpGet("{id}")] // åbent
        public async Task<ActionResult<PetDTO>> GetPet(int id)
        {
            var pet = await _petRepo.GetByIdAsync(id);
            if (pet == null) return NotFound();
            return PetToDTO(pet);
        }

        [HttpPost] // JWT + ejerskab/admin
        [Authorize]
        public async Task<ActionResult<PetDTO>> CreatePet([FromBody] PetDTO petDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!IsOwnerOrAdmin(petDto.OwnerId)) return Forbid();

            var pet = new Pet
            {
                OwnerId = petDto.OwnerId,
                Name = petDto.Name,
                Type = petDto.Type,
                Breed = petDto.Breed,
                ImageUrl = petDto.ImageUrl,
                Age = petDto.Age,
                Notes = petDto.Notes
            };

            if (petDto.CareInstruction != null)
            {
                pet.CareInstruction = new CareInstruction
                {
                    FoodAmountPerDay = petDto.CareInstruction.FoodAmountPerDay,
                    FoodType = petDto.CareInstruction.FoodType,
                    Likes = petDto.CareInstruction.Likes,
                    Dislikes = petDto.CareInstruction.Dislikes,
                    Notes = petDto.CareInstruction.Notes
                };
            }

            await _petRepo.AddAsync(pet);
            return CreatedAtAction(nameof(GetPet), new { id = pet.Id }, PetToDTO(pet));
        }

        [HttpPut("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> UpdatePet(int id, [FromBody] PetDTO petDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pet = await _petRepo.GetByIdAsync(id);
            if (pet == null) return NotFound();
            if (!IsOwnerOrAdmin(pet.OwnerId)) return Forbid();

            pet.Name = petDto.Name;
            pet.Type = petDto.Type;
            pet.Breed = petDto.Breed;
            pet.ImageUrl = petDto.ImageUrl;
            pet.Age = petDto.Age;
            pet.Notes = petDto.Notes;

            if (petDto.CareInstruction != null)
            {
                pet.CareInstruction ??= new CareInstruction();
                pet.CareInstruction.FoodAmountPerDay = petDto.CareInstruction.FoodAmountPerDay;
                pet.CareInstruction.FoodType = petDto.CareInstruction.FoodType;
                pet.CareInstruction.Likes = petDto.CareInstruction.Likes;
                pet.CareInstruction.Dislikes = petDto.CareInstruction.Dislikes;
                pet.CareInstruction.Notes = petDto.CareInstruction.Notes;
            }

            await _petRepo.UpdateAsync(pet);
            return NoContent();
        }

        [HttpDelete("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _petRepo.GetByIdAsync(id);
            if (pet == null) return NotFound();
            if (!IsOwnerOrAdmin(pet.OwnerId)) return Forbid();

            await _petRepo.DeleteAsync(pet);
            return NoContent();
        }

        private bool IsOwnerOrAdmin(int ownerId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);
            return userId == ownerId || role == "Admin";
        }

        private static PetDTO PetToDTO(Pet pet) => new PetDTO
        {
            Id = pet.Id,
            OwnerId = pet.OwnerId,
            OwnerName = pet.Owner?.Name,
            Name = pet.Name,
            Type = pet.Type,
            Breed = pet.Breed,
            ImageUrl = pet.ImageUrl,
            Age = pet.Age,
            Notes = pet.Notes,
            CareInstruction = pet.CareInstruction != null ? new CareInstructionDTO
            {
                Id = pet.CareInstruction.Id,
                PetId = pet.CareInstruction.PetId,
                FoodAmountPerDay = pet.CareInstruction.FoodAmountPerDay,
                FoodType = pet.CareInstruction.FoodType,
                Likes = pet.CareInstruction.Likes,
                Dislikes = pet.CareInstruction.Dislikes,
                Notes = pet.CareInstruction.Notes
            } : null,
            CarePeriods = pet.CarePeriods?.Select(cp => new CarePeriodDTO
            {
                Id = cp.Id,
                PetId = cp.PetId,
                StartDate = cp.StartDate,
                EndDate = cp.EndDate,
                SitterId = cp.SitterId,
                SitterName = cp.Sitter?.Name,
                Status = cp.Status
            }).ToList() ?? new List<CarePeriodDTO>()
        };
    }
}