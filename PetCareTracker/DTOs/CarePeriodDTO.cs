namespace PetCareTracker.DTOs
{
    public class CarePeriodDTO
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SitterId { get; set; }
        public string? SitterName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
