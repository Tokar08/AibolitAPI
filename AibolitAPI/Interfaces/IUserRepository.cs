using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<Role> GetRoleByNameAsync(string roleName);
}