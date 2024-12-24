using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IWorkScheduleRepository : IRepository<WorkSchedule>
{
    Task<List<WorkSchedule>> GetByIdsAsync(IEnumerable<Guid> ids);
}