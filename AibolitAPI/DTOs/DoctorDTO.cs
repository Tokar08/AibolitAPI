namespace AibolitAPI.DTOs;

public class DoctorDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserDTO? User { get; set; }
    public string Specialization { get; set; }
    public Guid WorkScheduleId { get; set; }
    public WorkScheduleDTO? WorkSchedule { get; set; }
    public Guid HospitalId { get; set; }
    public HospitalDTO? Hospital { get; set; }
    public int VisitCount { get; set; }
    public ICollection<PatientDTO>? Patients { get; set; }
    public ICollection<PatientDTO>? LikedByPatients { get; set; }
    public bool IsActive { get; set; }
}