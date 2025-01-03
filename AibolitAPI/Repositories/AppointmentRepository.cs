using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    private readonly AibolitDbContext _context;

    public AppointmentRepository(AibolitDbContext context) : base(context)
    {
        _context = context;
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

    public async Task<List<Appointment>> GetUpcomingAppointmentsAsync()
    {
        var now = DateTime.UtcNow;

        return await _context.Appointments
            .Where(a => a.AppointmentDate >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
    {
        var now = DateTime.UtcNow;

        var appointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.Doctor.IsActive && a.AppointmentDate > now && a.IsScheduled)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();

        var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
        foreach (var appointment in appointments)
            appointment.AppointmentDate = TimeZoneInfo.ConvertTimeFromUtc(appointment.AppointmentDate, kievTimeZone);

        return appointments;
    }


    public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
    {
        var appointments = await _context.Set<Appointment>()
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
        foreach (var appointment in appointments)
            appointment.AppointmentDate = TimeZoneInfo.ConvertTimeFromUtc(appointment.AppointmentDate, kievTimeZone);

        return appointments;
    }


    public async Task CancelAppointmentAsync(Guid appointmentId)
    {
        var appointment = await GetByIdAsync(appointmentId);
        if (appointment == null)
            throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");

        if (!appointment.IsScheduled)
            throw new InvalidOperationException("Appointment is not scheduled or already cancelled.");

        appointment.IsScheduled = false;
        await _context.SaveChangesAsync();
    }
}