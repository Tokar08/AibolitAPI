namespace AibolitAPI.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    
    public string Address { get; set; }
    
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}