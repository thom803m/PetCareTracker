using Microsoft.AspNetCore.Mvc;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarePeriodsController : ControllerBase
    {
        private readonly ICarePeriodRepository _cpRepo;

        public CarePeriodsController(ICarePeriodRepository cpRepo)
        {
            _cpRepo = cpRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarePeriodDTO>>> GetAll()
        {
            var periods = await _cpRepo.GetAllAsync();
            return Ok(periods.Select(p => new CarePeriodDTO
            {
                Id = p.Id,
                PetId = p.PetId,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                SitterId = p.SitterId,
                Status = p.Status
            }));
        }

        [HttpPost]
        public async Task<ActionResult<CarePeriodDTO>> Create([FromBody] CarePeriodDTO dto)
        {
            var cp = new CarePeriod
            {
                PetId = dto.PetId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                SitterId = dto.SitterId,
                Status = dto.Status
            };
            await _cpRepo.AddAsync(cp);
            dto.Id = cp.Id;

            return CreatedAtAction(nameof(GetAll), new { id = cp.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CarePeriodDTO dto)
        {
            var cp = await _cpRepo.GetByIdAsync(id);
            if (cp == null) return NotFound("Care period not found");

            cp.PetId = dto.PetId;
            cp.StartDate = dto.StartDate;
            cp.EndDate = dto.EndDate;
            cp.SitterId = dto.SitterId;
            cp.Status = dto.Status;

            await _cpRepo.UpdateAsync(cp);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cp = await _cpRepo.GetByIdAsync(id);
            if (cp == null) return NotFound("Care period not found");

            await _cpRepo.DeleteAsync(cp);
            return NoContent();
        }
    }
}
