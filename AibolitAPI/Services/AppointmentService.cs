using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace AibolitAPI.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorService _doctorService;
    private readonly ILogger<AppointmentService> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationSender _notificationSender;
    private readonly IPatientService _patientService;
    private readonly IEmailTemplateFactory _templateFactory;

    public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper,
        ILogger<AppointmentService> logger, IPatientService patientService, INotificationSender notificationSender,
        IEmailTemplateFactory templateFactory, IDoctorService doctorService)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
        _patientService = patientService;
        _notificationSender = notificationSender;
        _templateFactory = templateFactory;
        _doctorService = doctorService;
    }

    public async Task<IEnumerable<AppointmentDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync(page, size
            );

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
            var patientJson = await _patientService.GetByIdAsync(appointmentDto.PatientId);
            var patient = JsonConvert.DeserializeObject<PatientDTO>(patientJson);

            var doctorJson = await _doctorService.GetByIdAsync(appointmentDto.DoctorId);
            var doctor = JsonConvert.DeserializeObject<DoctorDTO>(doctorJson);

            var template = _templateFactory.GetTemplate("appointment_confirmation");
            await _notificationSender.SendAsync(
                patient.Email,
                template,
                new object[]
                {
                    patient.FirstName, doctor.FirstName + " " + doctor.LastName,
                    appointmentDto.AppointmentDate.ToString("dd.MM.yyyy HH:mm"),
                    doctor.Email, doctor.PhoneNumber
                }
            );

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