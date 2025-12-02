using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.DTOs;
using PetCareTracker.Models;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PetCareDbContext _context;

        public UsersController(PetCareDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Pets)
                .ThenInclude(p => p.CareInstruction)
                .ToListAsync();

            var userDTOs = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Pets = u.Pets.Select(p => new PetDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Type = p.Type,
                    Breed = p.Breed,
                    ImageUrl = p.ImageUrl,
                    Age = p.Age,
                    Notes = p.Notes,
                    CareInstruction = p.CareInstruction != null ? new CareInstructionDTO
                    {
                        FoodAmountPerDay = p.CareInstruction.FoodAmountPerDay,
                        FoodType = p.CareInstruction.FoodType ?? string.Empty,
                        Likes = p.CareInstruction.Likes ?? string.Empty,
                        Dislikes = p.CareInstruction.Dislikes ?? string.Empty,
                        Notes = p.CareInstruction.Notes ?? string.Empty
                    } : null
                }).ToList()
            }).ToList();

            return Ok(userDTOs);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Pets)
                .ThenInclude(p => p.CareInstruction)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Pets = user.Pets.Select(p => new PetDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Type = p.Type,
                    Breed = p.Breed,
                    ImageUrl = p.ImageUrl,
                    Age = p.Age,
                    Notes = p.Notes,
                    CareInstruction = p.CareInstruction != null ? new CareInstructionDTO
                    {
                        FoodAmountPerDay = p.CareInstruction.FoodAmountPerDay,
                        FoodType = p.CareInstruction.FoodType ?? string.Empty,
                        Likes = p.CareInstruction.Likes ?? string.Empty,
                        Dislikes = p.CareInstruction.Dislikes ?? string.Empty,
                        Notes = p.CareInstruction.Notes ?? string.Empty
                    } : null
                }).ToList()
            };

            return Ok(userDTO);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDTO)
        {
            var user = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDTO.Id = user.Id;

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDTO);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.Id) return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = userDTO.Name;
            user.Email = userDTO.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}