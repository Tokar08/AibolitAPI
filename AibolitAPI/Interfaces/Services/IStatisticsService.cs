using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsDTO> GetStatisticsAsync(Guid hospitalId);
}