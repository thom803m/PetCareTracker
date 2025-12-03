using System.ComponentModel.DataAnnotations;

namespace PetCareTracker.DTOs
{
    public class PetDTO
    {
        public int Id { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public string? OwnerName { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Breed { get; set; } = string.Empty;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, 100)]
        public int Age { get; set; }

        public string Notes { get; set; } = string.Empty;

        public CareInstructionDTO? CareInstruction { get; set; }

        public List<CarePeriodDTO> CarePeriods { get; set; } = new();
    }
}
