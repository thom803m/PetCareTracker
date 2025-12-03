using System.ComponentModel.DataAnnotations;

namespace PetCareTracker.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        public string? Role { get; set; } = "User";
    }
}
