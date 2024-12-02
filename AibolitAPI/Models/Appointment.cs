using Newtonsoft.Json;

namespace AibolitAPI.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    [JsonIgnore] public virtual Patient Patient { get; set; }
    public Guid DoctorId { get; set; }
    [JsonIgnore] public virtual Doctor Doctor { get; set; }
    public Guid MedicalRecordId { get; set; }
    [JsonIgnore] public virtual MedicalRecord MedicalRecord { get; set; }
    public DateTime AppointmentDate { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsActive { get; set; }
}