namespace PetCareTracker.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string ImageUrl { get; set; }
        public int Age { get; set; }
        public string Notes { get; set; }
        public CareInstruction CareInstruction { get; set; }
        public ICollection<CarePeriod> CarePeriods { get; set; }
    }
}
