using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IMedicalRecordService
{
    Task<IEnumerable<MedicalRecordDTO>> GetAllAsync(int page, int size);
    Task<MedicalRecordDTO> GetByIdAsync(Guid id);
    Task CreateAsync(MedicalRecordDTO medicalRecordDto);
    Task UpdateAsync(MedicalRecordDTO medicalRecordDto);
    Task SoftDeleteAsync(Guid id);
}