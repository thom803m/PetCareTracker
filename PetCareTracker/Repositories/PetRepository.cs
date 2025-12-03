using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

namespace PetCareTracker.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetCareDbContext _context;
        public PetRepository(PetCareDbContext context) => _context = context;

        public async Task<IEnumerable<Pet>> GetAllAsync() =>
            await _context.Pets.Include(p => p.Owner)
                               .Include(p => p.CareInstruction)
                               .Include(p => p.CarePeriods).ThenInclude(cp => cp.Sitter)
                               .ToListAsync();

        public async Task<Pet?> GetByIdAsync(int id) =>
            await _context.Pets.Include(p => p.Owner)
                               .Include(p => p.CareInstruction)
                               .Include(p => p.CarePeriods).ThenInclude(cp => cp.Sitter)
                               .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Pet> AddAsync(Pet pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task UpdateAsync(Pet pet)
        {
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Pet pet)
        {
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }
    }
}