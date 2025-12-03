using Microsoft.EntityFrameworkCore;
using PetCareTracker.Data;
using PetCareTracker.Models;
using PetCareTracker.Repositories.Interfaces;

namespace PetCareTracker.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PetCareDbContext _context;
        public UserRepository(PetCareDbContext context) => _context = context;

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.Include(u => u.Pets).ThenInclude(p => p.CareInstruction).ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.Include(u => u.Pets).ThenInclude(p => p.CareInstruction)
                                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}