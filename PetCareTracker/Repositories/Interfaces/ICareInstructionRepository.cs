using PetCareTracker.Models;

namespace PetCareTracker.Repositories.Interfaces
{
    public interface ICareInstructionRepository
    {
        Task<IEnumerable<CareInstruction>> GetAllAsync();
        Task<CareInstruction?> GetByIdAsync(int id);
        Task<CareInstruction> AddAsync(CareInstruction ci);
        Task UpdateAsync(CareInstruction ci);
        Task DeleteAsync(CareInstruction ci);
    }
}