namespace PetCareTracker.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PetDTO> Pets { get; set; } = new(); // Kan være tom liste
    }
}
