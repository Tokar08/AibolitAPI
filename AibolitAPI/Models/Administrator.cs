namespace AibolitAPI.Models;

public class Administrator : HospitalStaff
{
    public Guid ManagedHospitalId { get; set; }
    public virtual Hospital ManagedHospital { get; set; }
    
    public virtual ICollection<Doctor> Doctors { get; set; }
    public virtual ICollection<Patient> Patients { get; set; }
    
    public bool IsActive { get; set; }
}