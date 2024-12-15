using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDTO>> GetAllAsync(int page, int size);
    Task<string> GetAllDoctorsWithSSOAsync(int page, int size);
    Task<string> GetByIdAsync(Guid id);
    Task<string> CreateWithPhotoAsync(DoctorDTO doctorDto, Stream? photoStream, string? fileName);
    Task<string> UpdateWithPhotoAsync(Guid id, DoctorDTO doctorDto, Stream? photoStream, string? fileName);
    Task SoftDeleteAsync(Guid id);
}