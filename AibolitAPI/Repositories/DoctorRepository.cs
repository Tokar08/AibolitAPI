using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class DoctorRepository(AibolitDbContext context) : Repository<Doctor>(context), IDoctorRepository
{
    public async Task<IEnumerable<Doctor>> GetDoctorsByUserIdsAsync(List<Guid> userIds)
    {
        return await _context.Doctors
            .Where(doctor => userIds.Contains(doctor.UserId))
            .ToListAsync();
    }

    public override async Task<IEnumerable<Doctor>> GetAllAsync(int page, int size)
    {
        return await _context.Doctors
            .Include(d => d.Specialization)
            .Include(d => d.WorkSchedules)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public override async Task<Doctor> GetByIdAsync(Guid id)
    {
        return await _context.Doctors
                   .Include(d => d.Specialization)
                   .Include(d => d.WorkSchedules)
                   .FirstOrDefaultAsync(d => d.Id == id)
               ?? throw new KeyNotFoundException($"Doctor with ID {id} not found.");
    }
}