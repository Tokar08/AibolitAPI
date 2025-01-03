using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDTO>> GetAllAsync(int page, int size);
    Task<string> GetAllDoctorsWithSSOAsync(int page, int size, DoctorFilterDTO? filterDto = null);
    Task<string> GetByIdAsync(Guid id);
    Task CreateAsync(DoctorDTO doctorDto, string keycloakId, Stream? photoStream, string? fileName);
    Task<string> UpdateWithPhotoAsync(Guid id, DoctorDTO doctorDto, Stream? photoStream, string? fileName);
    Task SoftDeleteAsync(Guid id);
    Task<string> GetPatientsForDoctorAsync(Guid doctorId, int page, int size, PatientFilterDTO? filterDto);
    Task<string> GetPatientByIdAsync(Guid doctorId, Guid patientId);
    Task<string> GetPrescriptionsForPatientAsync(Guid doctorId, Guid patientId, PrescriptionFilterDTO? filterDto);
    Task<string> GetRecommendationsForPatientAsync(Guid doctorId, Guid patientId, RecommendationFilterDTO? filterDto);
    Task<object> GetPatientPrescriptionByIdAsync(Guid doctorId, Guid patientId, Guid prescriptionId);
    Task<object> GetPatientRecommendationByIdAsync(Guid doctorId, Guid patientId, Guid recommendationId);

    Task<IEnumerable<AppointmentDTO>> GetScheduledAppointmentsByDoctorIdAsync(Guid doctorId,
        AppointmentFilterDTO? filterDto, int page, int size);

    Task CancelAppointmentAsync(Guid doctorId, Guid appointmentId);
    Task UpdateDoctorSchedulesAsync(Guid doctorId, IEnumerable<WorkScheduleDTO> scheduleDtos);
    Task<List<WorkScheduleDTO>> GetWorkSchedulesForDoctorAsync(Guid doctorId);
}