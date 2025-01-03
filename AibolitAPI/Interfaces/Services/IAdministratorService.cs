using AibolitAPI.DTOs;

namespace AibolitAPI.Interfaces;

public interface IAdministratorService
{
    Task<IEnumerable<AdministratorDTO>> GetAllAsync(int page, int size);
    Task<string> GetAllAdministratorWithSSOAsync(int page, int size);
    Task<string> GetByIdAsync(Guid id);
    Task CreateAsync(AdministratorDTO administratorDto, string keycloakId);
    Task UpdateAsync(AdministratorDTO administratorDto);
    Task SoftDeleteAsync(Guid id);
}