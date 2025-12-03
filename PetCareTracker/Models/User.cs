namespace PetCareTracker.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public ICollection<Pet> Pets { get; set; }
    }
}
