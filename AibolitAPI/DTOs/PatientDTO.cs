namespace AibolitAPI.DTOs;

public class PatientDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MedicalRecordId { get; set; }
    public ICollection<DoctorDTO>? Doctors { get; set; }
    public ICollection<DoctorDTO>? LikedDoctors { get; set; }
    public bool IsActive { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? BirthDate { get; set; }
}