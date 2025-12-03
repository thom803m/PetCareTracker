using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

namespace PetCareTracker.Repositories
{
    public class CarePeriodRepository : ICarePeriodRepository
    {
        private readonly PetCareDbContext _context;
        public CarePeriodRepository(PetCareDbContext context) => _context = context;

        public async Task<IEnumerable<CarePeriod>> GetAllAsync() =>
            await _context.CarePeriods.Include(cp => cp.Pet).Include(cp => cp.Sitter).ToListAsync();

        public async Task<CarePeriod?> GetByIdAsync(int id) =>
            await _context.CarePeriods.Include(cp => cp.Pet).Include(cp => cp.Sitter)
                                      .FirstOrDefaultAsync(cp => cp.Id == id);

        public async Task<CarePeriod> AddAsync(CarePeriod cp)
        {
            _context.CarePeriods.Add(cp);
            await _context.SaveChangesAsync();
            return cp;
        }

        public async Task UpdateAsync(CarePeriod cp)
        {
            _context.CarePeriods.Update(cp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CarePeriod cp)
        {
            _context.CarePeriods.Remove(cp);
            await _context.SaveChangesAsync();
        }
    }
}
