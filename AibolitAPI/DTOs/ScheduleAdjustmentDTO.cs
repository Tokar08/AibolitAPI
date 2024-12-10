namespace AibolitAPI.DTOs;

public class ScheduleAdjustmentDTO
{
    public Guid Id { get; set; }
    public Guid WorkScheduleId { get; set; }
    public DateTime SpecificDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
}