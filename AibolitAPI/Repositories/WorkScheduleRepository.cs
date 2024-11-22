using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;

namespace AibolitAPI.Repositories;

public class WorkScheduleRepository : Repository<WorkSchedule>, IWorkScheduleRepository
{
    public WorkScheduleRepository(AibolitDbContext context) : base(context)
    {
    }
}