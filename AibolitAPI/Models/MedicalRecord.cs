using Newtonsoft.Json;

namespace AibolitAPI.Models;

public class MedicalRecord
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }

    [JsonIgnore] public virtual Patient Patient { get; set; }

    public Guid? DoctorId { get; set; }

    [JsonIgnore] public virtual Doctor CreatedBy { get; set; }

    public DateTime RecordDate { get; set; }

    [JsonIgnore] public virtual ICollection<Appointment> Appointments { get; set; }

    [JsonIgnore] public virtual ICollection<Prescription> Prescriptions { get; set; }
    [JsonIgnore] public virtual ICollection<Recommendation> Recommendations { get; set; }
    public bool IsActive { get; set; }
}