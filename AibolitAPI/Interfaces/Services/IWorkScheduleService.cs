using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IWorkScheduleService
{
    Task<IEnumerable<WorkScheduleDTO>> GetAllAsync(int page, int size);
    Task<WorkScheduleDTO> GetByIdAsync(Guid id);
    Task CreateAsync(WorkScheduleDTO workScheduleDto);
    Task UpdateAsync(WorkScheduleDTO workScheduleDto);
    Task SoftDeleteAsync(Guid id);
}