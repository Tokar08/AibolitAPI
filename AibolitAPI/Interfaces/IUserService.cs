using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IUserService
{
    Task<User> LoginAsync(string username, string password);
    Task RegisterAsync(Patient patient);
}
