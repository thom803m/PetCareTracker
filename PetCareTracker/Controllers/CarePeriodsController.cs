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
    public class CarePeriodsController : ControllerBase
    {
        private readonly ICarePeriodRepository _cpRepo;
        private readonly IPetRepository _petRepo;

        public CarePeriodsController(ICarePeriodRepository cpRepo, IPetRepository petRepo)
        {
            _cpRepo = cpRepo;
            _petRepo = petRepo;
        }

        [HttpGet] // åbent
        public async Task<ActionResult<IEnumerable<CarePeriodDTO>>> GetCarePeriods()
        {
            var periods = await _cpRepo.GetAllAsync();
            return Ok(periods.Select(cp => new CarePeriodDTO
            {
                Id = cp.Id,
                PetId = cp.PetId,
                StartDate = cp.StartDate,
                EndDate = cp.EndDate,
                SitterId = cp.SitterId,
                SitterName = cp.Sitter?.Name,
                Status = cp.Status
            }).ToList());
        }

        [HttpPost] // JWT + ejerskab/admin
        [Authorize]
        public async Task<ActionResult<CarePeriodDTO>> CreateCarePeriod([FromBody] CarePeriodDTO petDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pet = await _petRepo.GetByIdAsync(petDto.PetId);
            if (pet == null) return BadRequest("Pet not found.");
            if (!IsOwnerOrAdmin(pet.OwnerId)) return Forbid();

            var cp = new CarePeriod
            {
                PetId = petDto.PetId,
                StartDate = petDto.StartDate,
                EndDate = petDto.EndDate,
                SitterId = petDto.SitterId,
                Status = petDto.Status
            };

            await _cpRepo.AddAsync(cp);
            petDto.Id = cp.Id;
            return CreatedAtAction(nameof(GetCarePeriods), new { id = cp.Id }, petDto);
        }

        [HttpPut("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> UpdateCarePeriod(int id, [FromBody] CarePeriodDTO petDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cp = await _cpRepo.GetByIdAsync(id);
            if (cp == null) return NotFound();
            if (!IsOwnerOrAdmin(cp.Pet.OwnerId)) return Forbid();

            cp.StartDate = petDto.StartDate;
            cp.EndDate = petDto.EndDate;
            cp.SitterId = petDto.SitterId;
            cp.Status = petDto.Status;

            await _cpRepo.UpdateAsync(cp);
            return NoContent();
        }

        [HttpDelete("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> DeleteCarePeriod(int id)
        {
            var cp = await _cpRepo.GetByIdAsync(id);
            if (cp == null) return NotFound();
            if (!IsOwnerOrAdmin(cp.Pet.OwnerId)) return Forbid();

            await _cpRepo.DeleteAsync(cp);
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