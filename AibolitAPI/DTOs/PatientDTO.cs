namespace AibolitAPI.DTOs;

public class PatientDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserDTO? User { get; set; }
    public Guid MedicalRecordId { get; set; }
    public MedicalRecordDTO? MedicalRecord { get; set; }
    public ICollection<DoctorDTO>? Doctors { get; set; }
    public ICollection<DoctorDTO>? LikedDoctors { get; set; }
    public bool IsActive { get; set; }
}