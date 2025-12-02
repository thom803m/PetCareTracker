using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.DTOs;
using PetCareTracker.Models;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareInstructionsController : ControllerBase
    {
        private readonly PetCareDbContext _context;

        public CareInstructionsController(PetCareDbContext context)
        {
            _context = context;
        }

        // GET: api/CareInstructions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CareInstructionDTO>>> GetCareInstructions()
        {
            var instructions = await _context.CareInstructions.ToListAsync();

            var dtoList = instructions.Select(ci => new CareInstructionDTO
            {
                Id = ci.Id,
                PetId = ci.PetId,
                FoodAmountPerDay = ci.FoodAmountPerDay,
                FoodType = ci.FoodType ?? string.Empty,
                Likes = ci.Likes ?? string.Empty,
                Dislikes = ci.Dislikes ?? string.Empty,
                Notes = ci.Notes ?? string.Empty
            }).ToList();

            return Ok(dtoList);
        }

        // GET: api/CareInstructions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CareInstructionDTO>> GetCareInstruction(int id)
        {
            var ci = await _context.CareInstructions.FindAsync(id);

            if (ci == null)
                return NotFound();

            var dto = new CareInstructionDTO
            {
                Id = ci.Id,
                PetId = ci.PetId,
                FoodAmountPerDay = ci.FoodAmountPerDay,
                FoodType = ci.FoodType ?? string.Empty,
                Likes = ci.Likes ?? string.Empty,
                Dislikes = ci.Dislikes ?? string.Empty,
                Notes = ci.Notes ?? string.Empty
            };

            return Ok(dto);
        }

        // POST: api/CareInstructions
        [HttpPost]
        public async Task<ActionResult<CareInstructionDTO>> CreateCareInstruction(CareInstructionDTO dto)
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

            _context.CareInstructions.Add(ci);
            await _context.SaveChangesAsync();

            dto.Id = ci.Id;

            return CreatedAtAction(nameof(GetCareInstruction), new { id = dto.Id }, dto);
        }

        // PUT: api/CareInstructions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCareInstruction(int id, CareInstructionDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var ci = await _context.CareInstructions.FindAsync(id);
            if (ci == null)
                return NotFound();

            ci.PetId = dto.PetId;
            ci.FoodAmountPerDay = dto.FoodAmountPerDay;
            ci.FoodType = dto.FoodType;
            ci.Likes = dto.Likes;
            ci.Dislikes = dto.Dislikes;
            ci.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CareInstructions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCareInstruction(int id)
        {
            var ci = await _context.CareInstructions.FindAsync(id);
            if (ci == null)
                return NotFound();

            _context.CareInstructions.Remove(ci);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
