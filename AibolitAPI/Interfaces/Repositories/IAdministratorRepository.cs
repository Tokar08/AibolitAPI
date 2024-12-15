using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IAdministratorRepository : IRepository<Administrator>
{
    Task<IEnumerable<Administrator>> GetAllAsync(int page, int size);
    Task<Administrator?> GetByIdAsync(Guid id);
    Task CreateAsync(Administrator administrator);
    Task UpdateAsync(Administrator administrator);
    Task SoftDeleteAsync(Guid id);
}