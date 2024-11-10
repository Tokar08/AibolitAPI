namespace AibolitAPI.Models;

public abstract class HospitalStaff
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}