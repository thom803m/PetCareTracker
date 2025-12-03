using PetCareTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace PetCareTracker.Data
{
    public class Seeder
    {
        private readonly PetCareDbContext _context;

        public Seeder(PetCareDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Seed Users
            if (!_context.Users.Any())
            {
                string defaultPassword = "Test1234!";
                string hash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

                var users = new List<User>
                {
                    new User { Id = 1, Name = "Nydia Westover", Email = "nwestover0@paginegialle.it", PasswordHash = hash },
                    new User { Id = 2, Name = "Maribelle Deguara", Email = "mdeguara1@wisc.edu", PasswordHash = hash },
                    new User { Id = 3, Name = "Tailor Smickle", Email = "tsmickle2@state.tx.us", PasswordHash = hash },
                    new User { Id = 4, Name = "Bank Lisciandro", Email = "blisciandro3@cmu.edu", PasswordHash = hash },
                    new User { Id = 5, Name = "Kylynn Tottman", Email = "ktottman4@stumbleupon.com", PasswordHash = hash },
                    new User { Id = 6, Name = "Darrel Halkyard", Email = "dhalkyard5@wiley.com", PasswordHash = hash },
                    new User { Id = 7, Name = "Kain Kinkaid", Email = "kkinkaid6@berkeley.edu", PasswordHash = hash },
                    new User { Id = 8, Name = "Aeriell MacEllen", Email = "amacellen7@hubpages.com", PasswordHash = hash },
                    new User { Id = 9, Name = "Jeanette Fattorini", Email = "jfattorini8@blogger.com", PasswordHash = hash },
                    new User { Id = 10, Name = "Jaine Beatey", Email = "jbeatey9@ebay.co.uk", PasswordHash = hash }
                };

                _context.Users.AddRange(users);
                _context.SaveChanges();
            }

            // Seed Pets
            if (!_context.Pets.Any())
            {
            var pets = new List<Pet>
                {
                    new Pet { Id=1, OwnerId=2, Name="Herman", Type="Cat", Breed="Abyssinier", ImageUrl="https://cdn.cnn.com/cnn/interactive/2019/09/style/cat-photographer-cnnphotos/media/05.jpg", Age=2, Notes="Loves sun spots and prefers to lie on the windowsill all day. Likes to be brushed, but only in short sessions"},
                    new Pet { Id=2, OwnerId=4, Name="Amalita", Type="Dog", Breed="Golden Retriever", ImageUrl="https://i.pinimg.com/564x/7f/26/e7/7f26e71b2c84e6b16d4f6d3fd8a58bca.jpg", Age=1, Notes="Loves to play with a ball and gets extra happy when you say her name in a bright tone. Has a little separation anxiety, so give her a few minutes to calm down"},
                    new Pet { Id=3, OwnerId=7, Name="Noemi", Type="Dog", Breed="Golden Retriever", ImageUrl="https://www.shutterstock.com/image-photo/happy-mongrel-dog-puppy-smiling-600nw-2662330321.jpg", Age=1, Notes="Is a big foodie, so make sure to keep snacks out of reach. Loves to be scratched behind the ears and often lies on his back to get a tummy rub"},
                    new Pet { Id=4, OwnerId=1, Name="Jania", Type="Cat", Breed="Chantilly", ImageUrl="https://images.axios.com/wn7JOEi7XvtB-NIhfj26yvfC4gI=/316x0:1036x720/1600x1600/2025/06/13/1749842458530.jpg", Age=3, Notes="Very curious and investigates everything new in the home. Purrs almost immediately when spoken to gently"},
                    new Pet { Id=5, OwnerId=8, Name="Emmalee", Type="Dog", Breed="Pembroke", ImageUrl="https://cdn05.zipify.com/RQZYw7dLmDEk4o6MjUoZpEqKAbE=/fit-in/3840x0/3ce46b1ccb124af29d7bc3744f975d5b/pot013-dec-blogs-61.jpeg", Age=2, Notes="Gets along really well with other dogs but can be a little shy around new people. Prefers long walks in the morning"},
                    new Pet { Id=6, OwnerId=5, Name="Imogen", Type="Dog", Breed="Golden Retriever", ImageUrl="https://imgcdn.stablediffusionweb.com/2025/1/1/5ec81855-90af-4716-b75d-d684bf2544d3.jpg", Age=2, Notes="Absolutely love the dolphins, especially the blue dolphin. Gets tired quickly in the heat, so short breaks on the walk are necessary"},
                    new Pet { Id=7, OwnerId=10, Name="Rikki", Type="Dog", Breed="Pug", ImageUrl="https://media-be.chewy.com/wp-content/uploads/2022/09/27100039/pug-cute-dogs.jpg", Age=3, Notes="Friendly and affectionate, but can sometimes forget his size and jump up to greet. Always sleeps with his paws up"},
                    new Pet { Id=8, OwnerId=3, Name="Cameron", Type="Dog", Breed="Bernese", ImageUrl="https://www.yourtango.com/sites/default/files/2020/bernese-mountain-dog.jpg", Age=3, Notes="Has a funny habit of picking up his own blankets and putting them close to you. Is very calm indoors, but gets super energetic in the garden"},
                    new Pet { Id=9, OwnerId=6, Name="Elladine", Type="Cat", Breed="Europé", ImageUrl="https://cdn.rescuegroups.org/2420/pictures/animals/21063/21063894/100553790.jpg", Age=2, Notes="Prefers to be petted on the cheeks and under the chin. Can be a little reserved with new people, but quickly becomes comfortable"},
                    new Pet { Id=10, OwnerId=9, Name="Flory", Type="Cat", Breed="Europé", ImageUrl="https://cdn.rescuegroups.org/2420/pictures/animals/21234/21234253/100620686.jpg", Age=3, Notes="Has a weakness for toy mice and chases them eagerly. Eats slowly, so give her time during meals"}
                };
                _context.Pets.AddRange(pets);
                _context.SaveChanges();
            }

            // Seed CareInstructions
            if (!_context.CareInstructions.Any())
            {
                var instructions = new List<CareInstruction>
                {
                    new CareInstruction{Id=1, PetId=4, FoodAmountPerDay=135, FoodType="Fish Taste", Likes="Loves belly rubs", Dislikes="Dislikes loud noises", Notes="Remember to give an extra treat after the walk"},
                    new CareInstruction{Id=2, PetId=5, FoodAmountPerDay=123, FoodType="Beef Taste", Likes="Enjoys short walks in the park", Dislikes="Doesn’t like strangers", Notes="Likes to sleep on the rug in the living room"},
                    new CareInstruction{Id=3, PetId=10, FoodAmountPerDay=254, FoodType="Chicken Taste", Likes="Likes playing with squeaky toys", Dislikes="Dislikes getting wet", Notes="Needs to take medication every morning at 8 AM"},
                    new CareInstruction{Id=4, PetId=3, FoodAmountPerDay=121, FoodType="Beef Taste", Likes="Prefers cuddling in the evening", Dislikes="Doesn’t enjoy long walks", Notes="Is a bit shy around strangers"},
                    new CareInstruction{Id=5, PetId=1, FoodAmountPerDay=85, FoodType="Chicken Taste", Likes="Enjoys chasing balls", Dislikes="Dislikes being left alone", Notes="Prefers to eat in the kitchen"},
                    new CareInstruction{Id=6, PetId=2, FoodAmountPerDay=145, FoodType="Beef Taste", Likes="Likes being brushed daily", Dislikes="Not a fan of car rides", Notes="Walks best off-leash in the yard"},
                    new CareInstruction{Id=7, PetId=8, FoodAmountPerDay=214, FoodType="Beef Taste", Likes="Enjoys treats after meals", Dislikes="Dislikes strong smells", Notes="Responds positively to toys that make noise"},
                    new CareInstruction{Id=8, PetId=9, FoodAmountPerDay=237, FoodType="Fish Taste", Likes="Loves playing with other pets", Dislikes="Doesn’t like bath time", Notes="Does best with shorter walks twice a day"},
                    new CareInstruction{Id=9, PetId=6, FoodAmountPerDay=97, FoodType="Chicken Taste", Likes="Likes swimming occasionally", Dislikes="Dislikes being picked up", Notes="Likes to be brushed after playing"},
                    new CareInstruction{Id=10, PetId=7, FoodAmountPerDay=177, FoodType="Chicken Taste", Likes="Enjoys sunbathing by the window", Dislikes="Gets scared by vacuum cleaners", Notes="Remember to close the window when you leave"}
                };
                _context.CareInstructions.AddRange(instructions);
                _context.SaveChanges();
            }

            // Seed CarePeriods
            if (!_context.CarePeriods.Any())
            {
                var periods = new List<CarePeriod>
                {
                    new CarePeriod { Id = 1, PetId = 7, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-02"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-05"), DateTimeKind.Utc), SitterId = 5, Status = "Completed" },
                    new CarePeriod { Id = 2, PetId = 8, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-01"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-03"), DateTimeKind.Utc), SitterId = 10, Status = "Completed" },
                    new CarePeriod { Id = 3, PetId = 2, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-04"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-07"), DateTimeKind.Utc), SitterId = 7, Status = "Assigned" },
                    new CarePeriod { Id = 4, PetId = 10, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-03"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-06"), DateTimeKind.Utc), SitterId = 9, Status = "Assigned" },
                    new CarePeriod { Id = 5, PetId = 4, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-02"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-04"), DateTimeKind.Utc), SitterId = 3, Status = "Completed" },
                    new CarePeriod { Id = 6, PetId = 1, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-05"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-07"), DateTimeKind.Utc), SitterId = 3, Status = "Assigned" },
                    new CarePeriod { Id = 7, PetId = 5, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-01"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-04"), DateTimeKind.Utc), SitterId = 4, Status = "Completed" },
                    new CarePeriod { Id = 8, PetId = 6, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-02"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-05"), DateTimeKind.Utc), SitterId = 5, Status = "Completed" },
                    new CarePeriod { Id = 9, PetId = 3, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-03"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-06"), DateTimeKind.Utc), SitterId = 1, Status = "Assigned" },
                    new CarePeriod { Id = 10, PetId = 9, StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-01"), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-03"), DateTimeKind.Utc), SitterId = 8, Status = "Completed" }
                };

                _context.CarePeriods.AddRange(periods);
                _context.SaveChanges();
            }

            Console.WriteLine("Seeding complete!");
        }
    }
}
