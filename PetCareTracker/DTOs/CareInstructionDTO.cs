namespace PetCareTracker.DTOs
{
    public class CareInstructionDTO
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public int FoodAmountPerDay { get; set; }
        public string FoodType { get; set; } = string.Empty;
        public string Likes { get; set; } = string.Empty;
        public string Dislikes { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
