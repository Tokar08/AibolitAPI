using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IPatientService
{
    Task<string> GetAllPatientsWithSSOAsync(int page, int pageSize);
    Task<IEnumerable<PatientDTO>> GetAllAsync(int page, int size);
    Task<string> GetByIdAsync(Guid id);
    Task CreateAsync(PatientDTO patientDto);
    Task UpdateAsync(PatientDTO patientDto);
    Task SoftDeleteAsync(Guid id);
    Task<string> GetAllDoctorsAsyncWithSSO(int page, int pageSize);
    Task AddDoctorToFavoritesAsync(Guid patientId, Guid doctorId);
    Task RemoveDoctorFromFavoritesAsync(Guid patientId, Guid doctorId);
    Task<string> GetFavoriteDoctorsWithSSOAsync(Guid patientId, int page, int size);
    Task<string> GetAllAppointmentsAsync(Guid patientId, int page, int size);
    Task<string> GetPrescriptionsForPatientAsync(Guid patientId);
    Task<object> GetPrescriptionByIdWithDoctorAsync(Guid patientId, Guid prescriptionId);
    Task<string> GetRecommendationsForPatientAsync(Guid patientId);
    Task<object> GetRecommendationByIdWithDoctorAsync(Guid patientId, Guid recommendationId);
    Task CancelAppointmentAsync(Guid patientId, Guid appointmentId);
    Task CreateAppointmentAsync(Guid doctorId, Guid patientId);
}