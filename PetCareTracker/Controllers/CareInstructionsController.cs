using Microsoft.AspNetCore.Mvc;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareInstructionsController : ControllerBase
    {
        private readonly ICareInstructionRepository _ciRepo;

        public CareInstructionsController(ICareInstructionRepository ciRepo)
        {
            _ciRepo = ciRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CareInstructionDTO>>> GetAll()
        {
            var items = await _ciRepo.GetAllAsync();
            return Ok(items.Select(ci => new CareInstructionDTO
            {
                Id = ci.Id,
                PetId = ci.PetId,
                FoodAmountPerDay = ci.FoodAmountPerDay,
                FoodType = ci.FoodType,
                Likes = ci.Likes,
                Dislikes = ci.Dislikes,
                Notes = ci.Notes
            }));
        }

        [HttpPost]
        public async Task<ActionResult<CareInstructionDTO>> Create([FromBody] CareInstructionDTO dto)
        {
            var ci = new CareInstruction
            {
                PetId = dto.PetId,
                FoodAmountPerDay = dto.FoodAmountPerDay,
                FoodType = dto.FoodType,
                Likes = dto.Likes,
                Dislikes = dto.Dislikes,
                Notes = dto.Notes
            };
            await _ciRepo.AddAsync(ci);
            dto.Id = ci.Id;

            return CreatedAtAction(nameof(GetAll), new { id = ci.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CareInstructionDTO dto)
        {
            var ci = await _ciRepo.GetByIdAsync(id);
            if (ci == null) return NotFound("Care instruction not found");

            ci.PetId = dto.PetId;
            ci.FoodAmountPerDay = dto.FoodAmountPerDay;
            ci.FoodType = dto.FoodType;
            ci.Likes = dto.Likes;
            ci.Dislikes = dto.Dislikes;
            ci.Notes = dto.Notes;

            await _ciRepo.UpdateAsync(ci);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ci = await _ciRepo.GetByIdAsync(id);
            if (ci == null) return NotFound("Care instruction not found");

            await _ciRepo.DeleteAsync(ci);
            return NoContent();
        }
    }
}
