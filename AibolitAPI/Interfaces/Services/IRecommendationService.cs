using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IRecommendationService
{
    Task<IEnumerable<RecommendationDTO>> GetAllAsync(int page, int size, RecommendationFilterDTO? filterDto);
    Task<object> GetByIdAsync(Guid id);
    Task CreateAsync(Guid doctorId, Guid patientId, RecommendationDTO recommendationDto);
    Task UpdateAsync(RecommendationDTO recommendationDto);
    Task SoftDeleteAsync(Guid id);
}