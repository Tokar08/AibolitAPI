using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ILogger<RecommendationService> _logger;
    private readonly IMapper _mapper;
    private readonly IRecommendationRepository _recommendationRepository;

    public RecommendationService(IRecommendationRepository recommendationRepository, IMapper mapper,
        ILogger<RecommendationService> logger)
    {
        _recommendationRepository = recommendationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RecommendationDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var recommendations = await _recommendationRepository.GetAllAsync(page, size
            );
            return _mapper.Map<IEnumerable<RecommendationDTO>>(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all recommendations.");
            throw;
        }
    }

    public async Task<RecommendationDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var recommendation = await _recommendationRepository.GetByIdAsync(id);
            return _mapper.Map<RecommendationDTO>(recommendation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting recommendation with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(RecommendationDTO recommendationDto)
    {
        try
        {
            var recommendation = _mapper.Map<Recommendation>(recommendationDto);
            await _recommendationRepository.CreateAsync(recommendation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating recommendation.");
            throw;
        }
    }

    public async Task UpdateAsync(RecommendationDTO recommendationDto)
    {
        try
        {
            var recommendation = _mapper.Map<Recommendation>(recommendationDto);
            await _recommendationRepository.UpdateAsync(recommendation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating recommendation.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _recommendationRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting recommendation with ID: {id}");
            throw;
        }
    }
}