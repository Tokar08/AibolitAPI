namespace AibolitAPI.Models;

public class Role
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public virtual ICollection<User> Users { get; set; }
}