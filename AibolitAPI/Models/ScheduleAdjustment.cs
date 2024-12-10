namespace AibolitAPI.Models;

public class ScheduleAdjustment
{
    public Guid Id { get; set; }
    public Guid WorkScheduleId { get; set; }
    public virtual WorkSchedule WorkSchedule { get; set; }
    public DateTime SpecificDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
}