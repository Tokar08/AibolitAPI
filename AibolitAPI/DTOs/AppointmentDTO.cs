namespace AibolitAPI.DTOs;

public class AppointmentDTO
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public PatientDTO? Patient { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorDTO? Doctor { get; set; }
    public Guid MedicalRecordId { get; set; }
    public MedicalRecordDTO? MedicalRecord { get; set; }
    public DateTime AppointmentDate { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsActive { get; set; }
}