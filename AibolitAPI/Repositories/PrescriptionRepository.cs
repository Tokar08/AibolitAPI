using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
{
    private readonly AibolitDbContext _context;

    public PrescriptionRepository(AibolitDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Prescription>> GetAllAsync(int page, int size)
    {
        return await _context.Prescriptions
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public Task<IEnumerable<Prescription>> GetAllAsync(int page, int size,
        Func<IQueryable<Prescription>, IQueryable<Prescription>>? include)
    {
        throw new NotImplementedException();
    }

    public async Task<Prescription?> GetByIdAsync(Guid id)
    {
        return await _context.Prescriptions
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Prescription prescription)
    {
        await _context.Prescriptions.AddAsync(prescription);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Prescription prescription)
    {
        var existingPrescription = await GetByIdAsync(prescription.Id);
        if (existingPrescription == null)
            throw new InvalidOperationException("Prescription not found.");

        _context.Entry(existingPrescription).CurrentValues.SetValues(prescription);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var prescription = await GetByIdAsync(id);
        if (prescription == null)
            throw new InvalidOperationException("Prescription not found.");

        prescription.IsActive = false;
        await _context.SaveChangesAsync();
    }
}