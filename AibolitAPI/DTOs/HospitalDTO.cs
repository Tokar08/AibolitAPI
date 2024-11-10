namespace AibolitAPI.DTOs;

public class HospitalDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public ICollection<DoctorDTO> Doctors { get; set; } 
    public ICollection<AdministratorDTO> Administrators { get; set; } 
    public bool IsActive { get; set; }
}
