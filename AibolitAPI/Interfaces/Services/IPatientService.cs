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
}