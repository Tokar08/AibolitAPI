namespace AibolitAPI.Models;

public class WorkSchedule
{
    public Guid Id { get; set; }
    public string DaysOfWeek { get; set; } 
    public TimeSpan StartTime { get; set; } 
    public TimeSpan EndTime { get; set; }
}