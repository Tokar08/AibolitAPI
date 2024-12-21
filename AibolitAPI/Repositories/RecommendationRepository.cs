using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class RecommendationRepository : Repository<Recommendation>, IRecommendationRepository
{
    private readonly AibolitDbContext _context;

    public RecommendationRepository(AibolitDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Recommendation>> GetAllAsync(int page, int size)
    {
        return await _context.Recommendations
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public Task<IEnumerable<Recommendation>> GetAllAsync(int page, int size,
        Func<IQueryable<Recommendation>, IQueryable<Recommendation>>? include)
    {
        throw new NotImplementedException();
    }

    public async Task<Recommendation?> GetByIdAsync(Guid id)
    {
        return await _context.Recommendations
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task CreateAsync(Recommendation recommendation)
    {
        await _context.Recommendations.AddAsync(recommendation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Recommendation recommendation)
    {
        var existingRecommendation = await GetByIdAsync(recommendation.Id);
        if (existingRecommendation == null)
            throw new InvalidOperationException("Recommendation not found.");

        _context.Entry(existingRecommendation).CurrentValues.SetValues(recommendation);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var recommendation = await GetByIdAsync(id);
        if (recommendation == null)
            throw new InvalidOperationException("Recommendation not found.");

        recommendation.IsActive = false;
        await _context.SaveChangesAsync();
    }
}