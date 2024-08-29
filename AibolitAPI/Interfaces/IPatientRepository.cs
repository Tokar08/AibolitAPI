using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<IEnumerable<Patient>> GetAllAsync(int page, int size);
    Task<Patient> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Patient patient);
    Task<IEnumerable<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId, int page, int size);
    Task<Patient> GetPatientForDoctorByIdAsync(Guid doctorId, Guid patientId);
}