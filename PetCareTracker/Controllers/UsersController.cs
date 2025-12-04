using Microsoft.AspNetCore.Mvc;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

namespace PetCareTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // GET ALL USERS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userRepo.GetAllAsync();

            return Ok(users.Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Pets = u.Pets.Select(p => new PetDTO
                {
                    Id = p.Id,
                    OwnerId = p.OwnerId,
                    Name = p.Name,
                    Type = p.Type,
                    Breed = p.Breed,
                    ImageUrl = p.ImageUrl,
                    Age = p.Age,
                    Notes = p.Notes
                }).ToList()
            }).ToList());
        }

        // GET USER BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var u = await _userRepo.GetByIdAsync(id);
            if (u == null) return NotFound();

            return Ok(new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Pets = u.Pets.Select(p => new PetDTO
                {
                    Id = p.Id,
                    OwnerId = p.OwnerId,
                    Name = p.Name,
                    Type = p.Type,
                    Breed = p.Breed,
                    ImageUrl = p.ImageUrl,
                    Age = p.Age,
                    Notes = p.Notes
                }).ToList()
            });
        }

        // CREATE USER
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(UserDTO dto)
        {
            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email
            };

            await _userRepo.AddAsync(newUser);

            dto.Id = newUser.Id;
            dto.Pets = new List<PetDTO>();

            return CreatedAtAction(nameof(GetUser), new { id = dto.Id }, dto);
        }

        // UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Name = dto.Name ?? user.Name;
            user.Email = dto.Email ?? user.Email;

            await _userRepo.UpdateAsync(user);
            return NoContent();
        }

        // DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepo.DeleteAsync(user);
            return NoContent();
        }
    }
}
