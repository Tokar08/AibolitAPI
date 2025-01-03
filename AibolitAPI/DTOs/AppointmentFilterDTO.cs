namespace AibolitAPI.DTOs;

public class AppointmentFilterDTO
{
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public bool? IsScheduled { get; set; }
    public bool? SortByDescending { get; set; }
}