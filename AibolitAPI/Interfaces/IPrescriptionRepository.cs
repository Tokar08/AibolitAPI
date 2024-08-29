using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IPrescriptionRepository : IRepository<Prescription>
{
    Task<Prescription> CreateAsync(Prescription prescription);
    Task DeleteAsync(Guid prescriptionId);
    Task UpdateAsync(Prescription prescription);
    Task<IEnumerable<Prescription>> GetAllByPatientIdAsync(Guid patientId, int page, int size);
    Task<Prescription> GetByIdAsync(Guid prescriptionId);
}
