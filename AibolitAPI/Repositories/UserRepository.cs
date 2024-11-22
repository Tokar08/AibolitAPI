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
}