using System.ComponentModel.DataAnnotations;

namespace PetCareTracker.DTOs
{
    public class CarePeriodDTO
    {
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? SitterId { get; set; }
        public string? SitterName { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
