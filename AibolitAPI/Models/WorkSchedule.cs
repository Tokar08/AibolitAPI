namespace AibolitAPI.Models;

public class WorkSchedule
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public bool IsActive { get; set; }
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();


    public virtual ICollection<ScheduleAdjustment> ScheduleAdjustments { get; set; } = new List<ScheduleAdjustment>();
}