using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AibolitDbContext _context;

    public AppointmentRepository(AibolitDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(int page, int size)
    {
        return await _context.Appointments
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public Task<IEnumerable<Appointment>> GetAllAsync(int page, int size,
        Func<IQueryable<Appointment>, IQueryable<Appointment>>? include)
    {
        throw new NotImplementedException();
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task CreateAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        var existingAppointment = await GetByIdAsync(appointment.Id);
        if (existingAppointment == null)
            throw new InvalidOperationException("Appointment not found.");

        _context.Entry(existingAppointment).CurrentValues.SetValues(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var appointment = await GetByIdAsync(id);
        if (appointment == null)
            throw new InvalidOperationException("Appointment not found.");

        appointment.IsActive = false;
        await _context.SaveChangesAsync();
    }
}