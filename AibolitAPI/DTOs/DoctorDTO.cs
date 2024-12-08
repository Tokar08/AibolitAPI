namespace AibolitAPI.DTOs;

public class DoctorDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Specialization { get; set; }
    public Guid WorkScheduleId { get; set; }

    public Guid HospitalId { get; set; }
    public int YearsOfExperience { get; set; }
    public string Education { get; set; }
    public string PhotoUrl { get; set; }

    public int VisitCount { get; set; }
    public ICollection<PatientDTO>? Patients { get; set; }
    public ICollection<PatientDTO>? LikedByPatients { get; set; }
    public bool IsActive { get; set; }
}