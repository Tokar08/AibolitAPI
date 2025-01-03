using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class WorkScheduleRepository(AibolitDbContext context)
    : Repository<WorkSchedule>(context), IWorkScheduleRepository
{
    public async Task CreateWorkScheduleAsync(WorkSchedule workSchedule, TimeZoneInfo timeZoneInfo)
    {
        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
        workSchedule.StartTime = TimeZoneInfo.ConvertTimeToUtc(today + workSchedule.StartTime, timeZoneInfo).TimeOfDay;
        workSchedule.EndTime = TimeZoneInfo.ConvertTimeToUtc(today + workSchedule.EndTime, timeZoneInfo).TimeOfDay;

        await _dbSet.AddAsync(workSchedule);
        await _context.SaveChangesAsync();
    }


    public async Task<List<WorkSchedule>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.WorkSchedules
            .Where(ws => ids.Contains(ws.Id))
            .ToListAsync();
    }

    public async Task<List<WorkSchedule>> GetSchedulesForDoctorAsync(Guid doctorId, DayOfWeek dayOfWeek)
    {
        return await _context.WorkSchedules
            .Include(ws => ws.Doctors)
            .Where(ws => ws.Doctors.Any(d => d.Id == doctorId) && ws.DayOfWeek == dayOfWeek && ws.IsActive)
            .ToListAsync();
    }
}