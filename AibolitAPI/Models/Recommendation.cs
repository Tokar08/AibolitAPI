namespace AibolitAPI.Models;

public class Recommendation
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    public Guid DoctorId { get; set; }
    public virtual Doctor GivenBy { get; set; }
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    public string Content { get; set; }
    public DateTime RecommendationDate { get; set; }
}