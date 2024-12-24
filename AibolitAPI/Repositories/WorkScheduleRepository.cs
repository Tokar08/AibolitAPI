using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Repositories;

public class WorkScheduleRepository(AibolitDbContext context)
    : Repository<WorkSchedule>(context), IWorkScheduleRepository
{
    public async Task<List<WorkSchedule>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.WorkSchedules
            .Where(ws => ids.Contains(ws.Id))
            .ToListAsync();
    }
}