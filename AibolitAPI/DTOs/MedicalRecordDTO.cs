namespace AibolitAPI.DTOs;

public class MedicalRecordDTO
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public PatientDTO? Patient { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorDTO? CreatedBy { get; set; }
    public DateTime RecordDate { get; set; }
    public ICollection<AppointmentDTO>? Appointments { get; set; }
    public ICollection<PrescriptionDTO>? Prescriptions { get; set; }
    public ICollection<RecommendationDTO>? Recommendations { get; set; }
    public bool IsActive { get; set; }
}