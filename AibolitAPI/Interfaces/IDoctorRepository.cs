using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<IEnumerable<Doctor>> GetAllAsync(int page, int size);
    Task<Doctor> GetByIdAsync(Guid id);
    Task CreateAsync(Doctor doctor);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Doctor doctor);
}