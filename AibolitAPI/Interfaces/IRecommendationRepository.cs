using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IRecommendationRepository : IRepository<Recommendation>
{
    Task<Recommendation> CreateAsync(Recommendation recommendation);
    Task DeleteAsync(Guid recommendationId);
    Task UpdateAsync(Recommendation recommendation);
    Task<IEnumerable<Recommendation>> GetAllByPatientIdAsync(Guid patientId, int page, int size);
    Task<Recommendation> GetByIdAsync(Guid recommendationId);
}