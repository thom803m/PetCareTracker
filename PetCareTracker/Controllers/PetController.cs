using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.DTOs;
using PetCareTracker.Models;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly PetCareDbContext _context;

        public PetsController(PetCareDbContext context)
        {
            _context = context;
        }

        // GET: api/Pets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetPets()
        {
            var pets = await _context.Pets
                .Include(p => p.Owner)
                .Include(p => p.CareInstruction)
                .Include(p => p.CarePeriods)
                    .ThenInclude(cp => cp.Sitter)
                .ToListAsync();

            return pets.Select(PetToDTO).ToList();
        }

        // GET: api/Pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PetDTO>> GetPet(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Owner)
                .Include(p => p.CareInstruction)
                .Include(p => p.CarePeriods)
                    .ThenInclude(cp => cp.Sitter)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                return NotFound();

            return PetToDTO(pet);
        }

        // POST: api/Pets
        [HttpPost]
        public async Task<ActionResult<PetDTO>> CreatePet(PetDTO petDTO)
        {
            var pet = new Pet
            {
                OwnerId = petDTO.OwnerId,
                Name = petDTO.Name,
                Type = petDTO.Type,
                Breed = petDTO.Breed,
                ImageUrl = petDTO.ImageUrl,
                Age = petDTO.Age,
                Notes = petDTO.Notes
            };

            if (petDTO.CareInstruction != null)
            {
                pet.CareInstruction = new CareInstruction
                {
                    FoodAmountPerDay = petDTO.CareInstruction.FoodAmountPerDay,
                    FoodType = petDTO.CareInstruction.FoodType,
                    Likes = petDTO.CareInstruction.Likes,
                    Dislikes = petDTO.CareInstruction.Dislikes,
                    Notes = petDTO.CareInstruction.Notes
                };
            }

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPet), new { id = pet.Id }, PetToDTO(pet));
        }

        // PUT: api/Pets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, PetDTO petDTO)
        {
            var pet = await _context.Pets
                .Include(p => p.CareInstruction)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                return NotFound();

            pet.Name = petDTO.Name;
            pet.Type = petDTO.Type;
            pet.Breed = petDTO.Breed;
            pet.ImageUrl = petDTO.ImageUrl;
            pet.Age = petDTO.Age;
            pet.Notes = petDTO.Notes;

            if (petDTO.CareInstruction != null)
            {
                pet.CareInstruction ??= new CareInstruction(); // sørger for, at CareInstruction ikke er null
                pet.CareInstruction.FoodAmountPerDay = petDTO.CareInstruction.FoodAmountPerDay;
                pet.CareInstruction.FoodType = petDTO.CareInstruction.FoodType;
                pet.CareInstruction.Likes = petDTO.CareInstruction.Likes;
                pet.CareInstruction.Dislikes = petDTO.CareInstruction.Dislikes;
                pet.CareInstruction.Notes = petDTO.CareInstruction.Notes;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Pets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return NotFound();

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Konverter Pet til PetDTO
        private static PetDTO PetToDTO(Pet pet)
        {
            return new PetDTO
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
}
