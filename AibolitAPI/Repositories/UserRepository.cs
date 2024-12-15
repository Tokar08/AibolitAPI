using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly AibolitDbContext _context;

    public UserRepository(AibolitDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Role> GetRoleByNameAsync(string roleName)
    {
        return (await _context.Roles.FirstOrDefaultAsync(r => r.Title == roleName))!;
    }

    public async Task<User> GetUserByKeycloakId(string keycloakId)
    {
        return await _context.Users
                   .Include(u => u.Role)
                   .FirstOrDefaultAsync(u => u.KeycloakId == keycloakId)
               ?? throw new Exception("User not found.");
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync(int page, int size)
    {
        return await _context.Users
            .Include(u => u.Role)
            .OrderBy(u => u.Role.Title)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }
}