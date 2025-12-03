using System.ComponentModel.DataAnnotations;

namespace PetCareTracker.DTOs
{
    public class CareInstructionDTO
    {
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [Range(0, 10000)]
        public int FoodAmountPerDay { get; set; }

        [Required]
        [StringLength(50)]
        public string FoodType { get; set; } = string.Empty;

        public string Likes { get; set; } = string.Empty;
        public string Dislikes { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
