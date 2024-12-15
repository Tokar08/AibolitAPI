using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class AdministratorRepository : IAdministratorRepository
{
    private readonly AibolitDbContext _context;

    public AdministratorRepository(AibolitDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Administrator>> GetAllAsync(int page, int size)
    {
        return await _context.Administrators
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public Task<IEnumerable<Administrator>> GetAllAsync(int page, int size,
        Func<IQueryable<Administrator>, IQueryable<Administrator>>? include)
    {
        throw new NotImplementedException();
    }

    public async Task<Administrator?> GetByIdAsync(Guid id)
    {
        return await _context.Administrators
            .FirstOrDefaultAsync(admin => admin.Id == id);
    }

    public async Task CreateAsync(Administrator administrator)
    {
        await _context.Administrators.AddAsync(administrator);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Administrator administrator)
    {
        var existingAdmin = await GetByIdAsync(administrator.Id);
        if (existingAdmin == null)
            throw new InvalidOperationException("Administrator not found.");

        _context.Entry(existingAdmin).CurrentValues.SetValues(administrator);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var admin = await GetByIdAsync(id);
        if (admin == null)
            throw new InvalidOperationException("Administrator not found.");

        admin.IsActive = false;
        await _context.SaveChangesAsync();
    }
}