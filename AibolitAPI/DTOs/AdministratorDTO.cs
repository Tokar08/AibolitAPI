namespace AibolitAPI.DTOs;

public class AdministratorDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ManagedHospitalId { get; set; }
    public ICollection<DoctorDTO>? Doctors { get; set; }
    public ICollection<PatientDTO>? Patients { get; set; }
    public bool IsActive { get; set; }
}