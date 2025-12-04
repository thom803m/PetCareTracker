namespace PetCareTracker.Models
{
    public class CarePeriod
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public Pet Pet { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SitterId { get; set; }
        public User Sitter { get; set; }
        public string Status { get; set; }
    }
}
