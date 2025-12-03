using PetCareTracker.Models;

namespace PetCareTracker.Repositories.Interfaces
{
    public interface ICarePeriodRepository
    {
        Task<IEnumerable<CarePeriod>> GetAllAsync();
        Task<CarePeriod?> GetByIdAsync(int id);
        Task<CarePeriod> AddAsync(CarePeriod cp);
        Task UpdateAsync(CarePeriod cp);
        Task DeleteAsync(CarePeriod cp);
    }
}