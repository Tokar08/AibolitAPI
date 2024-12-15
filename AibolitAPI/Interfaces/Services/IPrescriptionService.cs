using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IPrescriptionService
{
    Task<IEnumerable<PrescriptionDTO>> GetAllAsync(int page, int size);
    Task<PrescriptionDTO> GetByIdAsync(Guid id);
    Task CreateAsync(PrescriptionDTO prescriptionDto);
    Task UpdateAsync(PrescriptionDTO prescriptionDto);
    Task SoftDeleteAsync(Guid id);
}