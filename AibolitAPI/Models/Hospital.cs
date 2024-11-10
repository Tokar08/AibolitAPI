namespace AibolitAPI.Models;

public class Hospital
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public virtual ICollection<Doctor> Doctors { get; set; }
    public virtual ICollection<Administrator> Administrators { get; set; }

    public bool IsActive { get; set; }
}