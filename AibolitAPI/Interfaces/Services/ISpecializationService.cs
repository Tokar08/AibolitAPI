using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces.Services;

public interface ISpecializationService
{
    Task<SpecializationDTO?> GetSpecializationByTitleAsync(string title);
    Task<IEnumerable<SpecializationDTO>> GetAllSpecializationsAsync();
    Task<bool> IsFamilyDoctorAsync(Guid specializationId);
}