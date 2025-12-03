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
    public class CareInstructionsController : ControllerBase
    {
        private readonly ICareInstructionRepository _ciRepo;
        private readonly IPetRepository _petRepo;

        public CareInstructionsController(ICareInstructionRepository ciRepo, IPetRepository petRepo)
        {
            _ciRepo = ciRepo;
            _petRepo = petRepo;
        }

        [HttpGet] // åbent
        public async Task<ActionResult<IEnumerable<CareInstructionDTO>>> GetCareInstructions()
        {
            var items = await _ciRepo.GetAllAsync();
            return Ok(items.Select(ci => new CareInstructionDTO
            {
                Id = ci.Id,
                PetId = ci.PetId,
                FoodAmountPerDay = ci.FoodAmountPerDay,
                FoodType = ci.FoodType ?? string.Empty,
                Likes = ci.Likes ?? string.Empty,
                Dislikes = ci.Dislikes ?? string.Empty,
                Notes = ci.Notes ?? string.Empty
            }).ToList());
        }

        [HttpGet("{id}")] // åbent
        public async Task<ActionResult<CareInstructionDTO>> GetCareInstruction(int id)
        {
            var ci = await _ciRepo.GetByIdAsync(id);
            if (ci == null) return NotFound();
            return new CareInstructionDTO
            {
                Id = ci.Id,
                PetId = ci.PetId,
                FoodAmountPerDay = ci.FoodAmountPerDay,
                FoodType = ci.FoodType ?? string.Empty,
                Likes = ci.Likes ?? string.Empty,
                Dislikes = ci.Dislikes ?? string.Empty,
                Notes = ci.Notes ?? string.Empty
            };
        }

        [HttpPost] // JWT + ejerskab/admin
        [Authorize]
        public async Task<ActionResult<CareInstructionDTO>> CreateCareInstruction([FromBody] CareInstructionDTO petDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pet = await _petRepo.GetByIdAsync(petDto.PetId);
            if (pet == null) return BadRequest("Pet not found.");
            if (!IsOwnerOrAdmin(pet.OwnerId)) return Forbid();

            var ci = new CareInstruction
            {
                PetId = petDto.PetId,
                FoodAmountPerDay = petDto.FoodAmountPerDay,
                FoodType = petDto.FoodType,
                Likes = petDto.Likes,
                Dislikes = petDto.Dislikes,
                Notes = petDto.Notes
            };

            await _ciRepo.AddAsync(ci);
            petDto.Id = ci.Id;
            return CreatedAtAction(nameof(GetCareInstruction), new { id = ci.Id }, petDto);
        }

        [HttpPut("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> UpdateCareInstruction(int id, [FromBody] CareInstructionDTO petDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ci = await _ciRepo.GetByIdAsync(id);
            if (ci == null) return NotFound();
            if (!IsOwnerOrAdmin(ci.Pet.OwnerId)) return Forbid();

            ci.FoodAmountPerDay = petDto.FoodAmountPerDay;
            ci.FoodType = petDto.FoodType;
            ci.Likes = petDto.Likes;
            ci.Dislikes = petDto.Dislikes;
            ci.Notes = petDto.Notes;

            await _ciRepo.UpdateAsync(ci);
            return NoContent();
        }

        [HttpDelete("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> DeleteCareInstruction(int id)
        {
            var ci = await _ciRepo.GetByIdAsync(id);
            if (ci == null) return NotFound();
            if (!IsOwnerOrAdmin(ci.Pet.OwnerId)) return Forbid();

            await _ciRepo.DeleteAsync(ci);
            return NoContent();
        }

        private bool IsOwnerOrAdmin(int ownerId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);
            return userId == ownerId || role == "Admin";
        }
    }
}