using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IPrescriptionService
{
    Task<IEnumerable<PrescriptionDTO>> GetAllAsync(int page, int size, PrescriptionFilterDTO? filterDto);
    Task<object> GetByIdAsync(Guid id);
    Task CreateAsync(Guid doctorId, Guid patientId, PrescriptionDTO prescriptionDto);
    Task UpdateAsync(PrescriptionDTO prescriptionDto);
    Task SoftDeleteAsync(Guid id);
}