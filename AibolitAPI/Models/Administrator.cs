namespace AibolitAPI.Models;

public class Administrator : HospitalStaff
{
    public Guid ManagedHospitalId { get; set; }
    public virtual Hospital ManagedHospital { get; set; }

    public bool IsActive { get; set; }
}