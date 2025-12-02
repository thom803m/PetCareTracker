using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.DTOs;
using PetCareTracker.Models;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarePeriodsController : ControllerBase
    {
        private readonly PetCareDbContext _context;

        public CarePeriodsController(PetCareDbContext context)
        {
            _context = context;
        }

        // GET: api/CarePeriods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarePeriodDTO>>> GetCarePeriods()
        {
            var periods = await _context.CarePeriods.ToListAsync();

            var dtoList = periods.Select(cp => new CarePeriodDTO
            {
                Id = cp.Id,
                PetId = cp.PetId,
                SitterId = cp.SitterId,
                StartDate = cp.StartDate,
                EndDate = cp.EndDate,
                Status = cp.Status ?? string.Empty
            }).ToList();

            return Ok(dtoList);
        }

        // GET: api/CarePeriods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarePeriodDTO>> GetCarePeriod(int id)
        {
            var cp = await _context.CarePeriods.FindAsync(id);

            if (cp == null)
                return NotFound();

            var dto = new CarePeriodDTO
            {
                Id = cp.Id,
                PetId = cp.PetId,
                SitterId = cp.SitterId,
                StartDate = cp.StartDate,
                EndDate = cp.EndDate,
                Status = cp.Status ?? string.Empty
            };

            return Ok(dto);
        }

        // POST: api/CarePeriods
        [HttpPost]
        public async Task<ActionResult<CarePeriodDTO>> CreateCarePeriod(CarePeriodDTO dto)
        {
            var cp = new CarePeriod
            {
                PetId = dto.PetId,
                SitterId = dto.SitterId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status
            };

            _context.CarePeriods.Add(cp);
            await _context.SaveChangesAsync();

            dto.Id = cp.Id;

            return CreatedAtAction(nameof(GetCarePeriod), new { id = dto.Id }, dto);
        }

        // PUT: api/CarePeriods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCarePeriod(int id, CarePeriodDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var cp = await _context.CarePeriods.FindAsync(id);
            if (cp == null)
                return NotFound();

            cp.PetId = dto.PetId;
            cp.SitterId = dto.SitterId;
            cp.StartDate = dto.StartDate;
            cp.EndDate = dto.EndDate;
            cp.Status = dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CarePeriods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarePeriod(int id)
        {
            var cp = await _context.CarePeriods.FindAsync(id);
            if (cp == null)
                return NotFound();

            _context.CarePeriods.Remove(cp);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
