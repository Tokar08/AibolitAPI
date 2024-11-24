namespace AibolitAPI.Models;

public class User
{
    public Guid Id { get; set; }
    public string KeycloakId { get; set; }
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}