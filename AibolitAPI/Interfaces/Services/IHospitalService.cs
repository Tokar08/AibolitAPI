using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IHospitalService
{
    Task<IEnumerable<HospitalDTO>> GetAllAsync(int page, int size);
    Task<string> GetByIdAsync(Guid id);
    Task CreateAsync(HospitalDTO hospitalDto);
    Task UpdateAsync(HospitalDTO hospitalDto);
    Task SoftDeleteAsync(Guid id);

    Task<string> GetPatientsForDoctorAsync(Guid hospitalId, Guid doctorId, int page, int size,
        PatientFilterDTO? filterDto);

    Task<IEnumerable<PrescriptionDTO>> GetPatientPrescriptionsAsync(Guid hospitalId, Guid doctorId, Guid patientId,
        PrescriptionFilterDTO? filterDto);

    Task<IEnumerable<RecommendationDTO>> GetPatientRecommendationsAsync(Guid hospitalId, Guid doctorId, Guid patientId,
        RecommendationFilterDTO? filterDto);

    Task<string> GetPatientInfoAsync(Guid hospitalId, Guid doctorId, Guid patientId);
    Task<string> GetAllHospitalsWithDetailsAsync(int page, int pageSize);
    Task<string> GetDoctorWithPatientsAsync(Guid hospitalId, Guid doctorId, int page, int size);
    Task<string> GetDoctorsByHospitalIdAsync(Guid hospitalId, DoctorFilterDTO? filterDto, int page, int size);


    Task<object> GetPatientRecommendationByIdAsync(Guid hospitalId, Guid doctorId,
        Guid patientId, Guid recommendationId);

    Task<object> GetPatientPrescriptionByIdAsync(Guid hospitalId, Guid doctorId,
        Guid patientId, Guid prescriptionId);
}