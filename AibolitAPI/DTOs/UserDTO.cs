namespace AibolitAPI.DTOs;

public class UserDTO
{
    public Guid Id { get; set; }
    public string KeycloakId { get; set; }
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}