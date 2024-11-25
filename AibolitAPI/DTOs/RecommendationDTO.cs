namespace AibolitAPI.DTOs;

public class RecommendationDTO
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid MedicalRecordId { get; set; }
    public string Content { get; set; }
    public DateTime RecommendationDate { get; set; }
    public bool IsActive { get; set; }
}