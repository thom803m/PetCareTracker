namespace PetCareTracker.DTOs
{
    public class PetDTO
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Notes { get; set; } = string.Empty;
        // Ingen CareInstruction og CarePeriods her
    }
}
