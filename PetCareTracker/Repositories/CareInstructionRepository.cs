using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

namespace PetCareTracker.Repositories
{
    public class CareInstructionRepository : ICareInstructionRepository
    {
        private readonly PetCareDbContext _context;
        public CareInstructionRepository(PetCareDbContext context) => _context = context;

        public async Task<IEnumerable<CareInstruction>> GetAllAsync() =>
            await _context.CareInstructions.Include(ci => ci.Pet).ToListAsync();

        public async Task<CareInstruction?> GetByIdAsync(int id) =>
            await _context.CareInstructions.Include(ci => ci.Pet).FirstOrDefaultAsync(ci => ci.Id == id);

        public async Task<CareInstruction> AddAsync(CareInstruction ci)
        {
            _context.CareInstructions.Add(ci);
            await _context.SaveChangesAsync();
            return ci;
        }

        public async Task UpdateAsync(CareInstruction ci)
        {
            _context.CareInstructions.Update(ci);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CareInstruction ci)
        {
            _context.CareInstructions.Remove(ci);
            await _context.SaveChangesAsync();
        }
    }
}