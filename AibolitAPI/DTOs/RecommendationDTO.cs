namespace AibolitAPI.DTOs;

public class RecommendationDTO
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public PatientDTO? Patient { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorDTO? GivenBy { get; set; }
    public Guid MedicalRecordId { get; set; }
    public MedicalRecordDTO? MedicalRecord { get; set; }
    public string Content { get; set; }
    public DateTime RecommendationDate { get; set; }
    public bool IsActive { get; set; }
}