using Microsoft.AspNetCore.Mvc;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetPets()
        {
            var pets = await _petRepo.GetAllAsync();
            return Ok(pets.Select(p => new PetDTO
            {
                Id = p.Id,
                OwnerId = p.OwnerId,
                Name = p.Name,
                Type = p.Type,
                Breed = p.Breed,
                ImageUrl = p.ImageUrl,
                Age = p.Age,
                Notes = p.Notes
            }));
        }

        [HttpPost]
        public async Task<ActionResult<PetDTO>> CreatePet([FromBody] PetDTO dto)
        {
            var pet = new Pet
            {
                OwnerId = dto.OwnerId,
                Name = dto.Name,
                Type = dto.Type,
                Breed = dto.Breed,
                ImageUrl = dto.ImageUrl,
                Age = dto.Age,
                Notes = dto.Notes
            };
            await _petRepo.AddAsync(pet);
            dto.Id = pet.Id;

            return CreatedAtAction(nameof(GetPets), new { id = pet.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, [FromBody] PetDTO dto)
        {
            var pet = await _petRepo.GetByIdAsync(id);
            if (pet == null) return NotFound("Pet not found");

            pet.Name = dto.Name;
            pet.Type = dto.Type;
            pet.Breed = dto.Breed;
            pet.ImageUrl = dto.ImageUrl;
            pet.Age = dto.Age;
            pet.Notes = dto.Notes;
            pet.OwnerId = dto.OwnerId;

            await _petRepo.UpdateAsync(pet);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _petRepo.GetByIdAsync(id);
            if (pet == null) return NotFound("Pet not found");

            await _petRepo.DeleteAsync(pet);
            return NoContent();
        }
    }
}
