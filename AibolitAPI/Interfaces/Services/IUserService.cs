using AibolitAPI.DTOs;
using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IUserService
{
    Task<UserDTO> AuthenticateOrRegisterAsync(string keycloakId, string email, string userName, string birthDateClaim);
    Task<User> GetUserByKeycloakIdAsync(string keycloakId);
    Task<IEnumerable<UserDTO>> GetAllAsync(int page, int size);
    Task<IEnumerable<Dictionary<string, object>>> GetAllUsersFromKeycloakAsync();
    Task<IEnumerable<Dictionary<string, object>>> GetExistingUsersAsync();
    Task<IEnumerable<UserDTO>> GetUsersWithSSOAsync(int page, int size);
    Task<UserDTO> GetByIdAsync(Guid id);
    Task CreateAsync(UserDTO userDto);
    Task UpdateAsync(UserDTO userDto);
    Task SoftDeleteAsync(Guid id);
}