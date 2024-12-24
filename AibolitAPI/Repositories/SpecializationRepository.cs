using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class SpecializationRepository(AibolitDbContext context)
    : Repository<Specialization>(context), ISpecializationRepository
{
    public async Task<Specialization?> GetByTitleAsync(string title)
    {
        return await _context.Specializations
            .Where(s => s.IsActive)
            .FirstOrDefaultAsync(s => s.Title.ToLower() == title.ToLower());
    }
}