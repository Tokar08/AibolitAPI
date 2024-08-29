namespace AibolitAPI.Models;

public class Hospital
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public Guid ChiefDoctorId { get; set; }
    public virtual Doctor ChiefDoctor { get; set; }  
        
    public Guid? AdministratorId { get; set; }
    public virtual Administrator Administrator { get; set; }
        
    public virtual ICollection<Doctor> Staff { get; set; } 
    public bool IsActive { get; set; }
}