namespace AibolitAPI.Models;

public class MedicalRecord
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    public Guid DoctorId { get; set; }
    public virtual Doctor CreatedBy { get; set; }
    public DateTime RecordDate { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
    public virtual ICollection<Prescription> Prescriptions { get; set; }
    public virtual ICollection<Recommendation> Recommendations { get; set; }
    public bool IsActive { get; set; }
}