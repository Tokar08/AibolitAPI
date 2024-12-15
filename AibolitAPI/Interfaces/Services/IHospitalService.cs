using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IHospitalService
{
    Task<string> GetAllHospitalsWithDetailsAsync(int page, int pageSize);
    Task<IEnumerable<HospitalDTO>> GetAllAsync(int page, int size);
    Task<string> GetByIdAsync(Guid id);
    Task CreateAsync(HospitalDTO hospitalDto);
    Task UpdateAsync(HospitalDTO hospitalDto);
    Task SoftDeleteAsync(Guid id);
}