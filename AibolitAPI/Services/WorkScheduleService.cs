using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class WorkScheduleService : IWorkScheduleService
{
    private readonly ILogger<WorkScheduleService> _logger;
    private readonly IMapper _mapper;
    private readonly IWorkScheduleRepository _workScheduleRepository;

    public WorkScheduleService(IWorkScheduleRepository workScheduleRepository, IMapper mapper,
        ILogger<WorkScheduleService> logger)
    {
        _workScheduleRepository = workScheduleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<WorkScheduleDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var workSchedules = await _workScheduleRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<WorkScheduleDTO>>(workSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all work schedules.");
            throw;
        }
    }

    public async Task<WorkScheduleDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var workSchedule = await _workScheduleRepository.GetByIdAsync(id);
            return _mapper.Map<WorkScheduleDTO>(workSchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting work schedule with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(WorkScheduleDTO workScheduleDto)
    {
        try
        {
            var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
            var workSchedule = _mapper.Map<WorkSchedule>(workScheduleDto);
            await _workScheduleRepository.CreateWorkScheduleAsync(workSchedule, kievTimeZone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating work schedule.");
            throw;
        }
    }

    public async Task UpdateAsync(WorkScheduleDTO workScheduleDto)
    {
        try
        {
            var workSchedule = _mapper.Map<WorkSchedule>(workScheduleDto);
            await _workScheduleRepository.UpdateAsync(workSchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating work schedule.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _workScheduleRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting work schedule with ID: {id}");
            throw;
        }
    }
}