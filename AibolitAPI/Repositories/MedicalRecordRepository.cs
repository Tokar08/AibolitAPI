using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly AibolitDbContext _context;

    public MedicalRecordRepository(AibolitDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MedicalRecord>> GetAllAsync(int page, int size)
    {
        var medicalRecords = await _context.MedicalRecords
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return medicalRecords;
    }

    public async Task<IEnumerable<MedicalRecord>> GetAllAsync(int page, int size,
        Func<IQueryable<MedicalRecord>, IQueryable<MedicalRecord>>? include)
    {
        var query = _context.MedicalRecords.AsQueryable();

        if (include != null) query = include(query);

        var medicalRecords = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return medicalRecords;
    }


    public async Task<MedicalRecord?> GetByIdAsync(Guid id)
    {
        return await _context.MedicalRecords.IgnoreQueryFilters()
            .FirstOrDefaultAsync(record => record.Id == id);
    }

    public async Task CreateAsync(MedicalRecord medicalRecord)
    {
        await _context.MedicalRecords.AddAsync(medicalRecord);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MedicalRecord medicalRecord)
    {
        var existingRecord = await GetByIdAsync(medicalRecord.Id);
        if (existingRecord == null)
            throw new InvalidOperationException("MedicalRecord not found.");

        _context.Entry(existingRecord).CurrentValues.SetValues(medicalRecord);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var record = await GetByIdAsync(id);
        if (record == null)
            throw new InvalidOperationException("MedicalRecord not found.");

        record.IsActive = false;
        await _context.SaveChangesAsync();
    }
}