using Newtonsoft.Json;

namespace AibolitAPI.DTOs;

public class PatientDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MedicalRecordId { get; set; }
    [JsonIgnore] public ICollection<DoctorDTO>? Doctors { get; set; }
    [JsonIgnore] public ICollection<DoctorDTO>? LikedDoctors { get; set; }
    public bool IsActive { get; set; }
}