using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface ISpecializationRepository : IRepository<Specialization>
{
    Task<Specialization?> GetByTitleAsync(string title);
}