namespace PetCareTracker.Models
{
    public class CareInstruction
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public Pet? Pet { get; set; }
        public int FoodAmountPerDay { get; set; }
        public string FoodType { get; set; }
        public string Likes { get; set; }
        public string Dislikes { get; set; }
        public string Notes { get; set; }
    }
}
