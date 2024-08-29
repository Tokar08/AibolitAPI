using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IHospitalRepository : IRepository<Hospital>
{
    Task<Hospital> GetByIdAsync(Guid id);
    Task UpdateAsync(Hospital hospital);
}
