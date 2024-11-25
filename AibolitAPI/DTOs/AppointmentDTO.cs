namespace AibolitAPI.DTOs;

public class AppointmentDTO
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid MedicalRecordId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsActive { get; set; }
}