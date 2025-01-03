using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDTO>> GetAllAsync(AppointmentFilterDTO? filterDto, int page, int pageSize);
    Task<AppointmentDTO> GetByIdAsync(Guid id);
    Task CreateAsync(AppointmentDTO appointmentDto);
    Task UpdateAsync(AppointmentDTO appointmentDto);
    Task SoftDeleteAsync(Guid id);
}