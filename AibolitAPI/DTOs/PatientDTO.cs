namespace AibolitAPI.DTOs;

public class PatientDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}