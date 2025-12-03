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
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet] // åbent
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userRepo.GetAllAsync();
            return Ok(users.Select(UserToDTO).ToList());
        }

        [HttpGet("{id}")] // åbent
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            return UserToDTO(user);
        }

        [HttpPost] // JWT + Admin
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User 
            { 
                Name = userDTO.Name, 
                Email = userDTO.Email 
            };

            await _userRepo.AddAsync(user);

            userDTO.Id = user.Id;
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDTO);
        }

        [HttpPut("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != userDTO.Id) return BadRequest();
 
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            if (!IsSelfOrAdmin(user.Id)) return Forbid();

            user.Name = userDTO.Name;
            user.Email = userDTO.Email;

            await _userRepo.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")] // JWT + ejerskab/admin
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();
            if (!IsSelfOrAdmin(id)) return Forbid();

            await _userRepo.DeleteAsync(user);
            return NoContent();
        }

        private bool IsSelfOrAdmin(int userId)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);
            return currentUserId == userId || role == "Admin";
        }

        private static UserDTO UserToDTO(User u) => new UserDTO
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
        };
    }
}