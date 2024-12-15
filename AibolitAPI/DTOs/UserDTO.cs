namespace AibolitAPI.DTOs;

public class UserDTO
{
    public Guid Id { get; set; }
    public string KeycloakId { get; set; }
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? BirthDate { get; set; }
}