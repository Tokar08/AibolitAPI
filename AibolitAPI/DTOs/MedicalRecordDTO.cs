using Newtonsoft.Json;

namespace AibolitAPI.DTOs;

public class MedicalRecordDTO
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime RecordDate { get; set; }

    [JsonIgnore] public ICollection<AppointmentDTO>? Appointments { get; set; }

    [JsonIgnore] public ICollection<PrescriptionDTO>? Prescriptions { get; set; }

    [JsonIgnore] public ICollection<RecommendationDTO>? Recommendations { get; set; }

    public bool IsActive { get; set; }
}