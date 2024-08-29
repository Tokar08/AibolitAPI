namespace AibolitAPI.DTOs;

public class AppointmentDTO
{
    public Guid Id { get; set; }
    public string PatientFullName { get; set; }
    public string DoctorFullName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsActive { get; set; }
}