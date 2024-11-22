using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;


public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper, ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AppointmentDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all appointments.");
            throw;
        }
    }

    public async Task<AppointmentDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return _mapper.Map<AppointmentDTO>(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting appointment with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(AppointmentDTO appointmentDto)
    {
        try
        {
            var appointment = _mapper.Map<Appointment>(appointmentDto);
            await _appointmentRepository.CreateAsync(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating appointment.");
            throw;
        }
    }

    public async Task UpdateAsync(AppointmentDTO appointmentDto)
    {
        try
        {
            var appointment = _mapper.Map<Appointment>(appointmentDto);
            await _appointmentRepository.UpdateAsync(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating appointment.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _appointmentRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting appointment with ID: {id}");
            throw;
        }
    }
}