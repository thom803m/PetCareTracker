using Microsoft.EntityFrameworkCore;
using PetCareTracker.Models;

public class PetCareDbContext : DbContext
{
    public PetCareDbContext(DbContextOptions<PetCareDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<CareInstruction> CareInstructions { get; set; }
    public DbSet<CarePeriod> CarePeriods { get; set; }
}
