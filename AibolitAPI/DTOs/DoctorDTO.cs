namespace AibolitAPI.DTOs;

public class DoctorDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string SpecializationTitle { get; set; }
    public Guid HospitalId { get; set; }
    public int YearsOfExperience { get; set; }
    public string Education { get; set; }
    public string PhotoUrl { get; set; }

    public int VisitCount { get; set; }
    public ICollection<PatientDTO>? Patients { get; set; }
    public ICollection<PatientDTO>? LikedByPatients { get; set; }
    public ICollection<WorkScheduleDTO>? WorkSchedules { get; set; }

    public bool IsActive { get; set; }

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? BirthDate { get; set; }
}