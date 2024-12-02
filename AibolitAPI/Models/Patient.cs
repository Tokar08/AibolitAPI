using Newtonsoft.Json;

namespace AibolitAPI.Models;

public class Patient
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [JsonIgnore] public virtual User User { get; set; }
    public Guid MedicalRecordId { get; set; }
    [JsonIgnore] public virtual MedicalRecord MedicalRecord { get; set; }
    [JsonIgnore] public virtual ICollection<Doctor> Doctors { get; set; }
    [JsonIgnore] public virtual ICollection<Doctor> LikedDoctors { get; set; }
    public bool IsActive { get; set; }
}