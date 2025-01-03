using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IPatientService
{
    Task<string> GetAllPatientsWithSSOAsync(int page, int size, PatientFilterDTO? filterDto = null);
    Task<IEnumerable<PatientDTO>> GetAllAsync(int page, int size);
    Task<string> GetByIdAsync(Guid id);
    Task CreateAsync(PatientDTO patientDto);
    Task UpdateAsync(PatientDTO patientDto);
    Task SoftDeleteAsync(Guid id);
    Task<string> GetAllDoctorsAsyncWithSSO(DoctorFilterDTO? filterDto, int page, int pageSize);
    Task AddDoctorToFavoritesAsync(Guid patientId, Guid doctorId);
    Task RemoveDoctorFromFavoritesAsync(Guid patientId, Guid doctorId);
    Task<string> GetFavoriteDoctorsWithSSOAsync(Guid patientId, DoctorFilterDTO? filterDto, int page, int size);
    Task<string> GetAllAppointmentsAsync(Guid patientId, AppointmentFilterDTO? filterDto, int page, int size);
    Task<string> GetPrescriptionsForPatientAsync(Guid patientId, PrescriptionFilterDTO? filterDto);
    Task<object> GetPrescriptionByIdWithDoctorAsync(Guid patientId, Guid prescriptionId);
    Task<string> GetRecommendationsForPatientAsync(Guid patientId, RecommendationFilterDTO? filterDto);
    Task<object> GetRecommendationByIdWithDoctorAsync(Guid patientId, Guid recommendationId);
    Task CancelAppointmentAsync(Guid patientId, Guid appointmentId);
    Task CreateAppointmentAsync(Guid doctorId, Guid patientId, DateTime appointmentDate);
    Task<IEnumerable<object>> GetAvailableSlotsAsync(Guid doctorId, DateTime date);
}