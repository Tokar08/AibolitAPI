using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly AibolitDbContext _context;

    public PatientRepository(AibolitDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(int page, int size)
    {
        return await _context.Patients
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(int page, int size,
        Func<IQueryable<Patient>, IQueryable<Patient>>? include)
    {
        var query = _context.Patients.AsQueryable();

        query = query.Include(p => p.User);

        if (include != null) query = include(query);

        var patients = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return patients;
    }


    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Patient patient)
    {
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient patient)
    {
        var existingPatient = await GetByIdAsync(patient.Id);
        if (existingPatient == null)
            throw new InvalidOperationException("Patient not found.");

        _context.Entry(existingPatient).CurrentValues.SetValues(patient);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var patient = await GetByIdAsync(id);
        if (patient == null)
            throw new InvalidOperationException("Patient not found.");

        patient.IsActive = false;
        await _context.SaveChangesAsync();
    }
}