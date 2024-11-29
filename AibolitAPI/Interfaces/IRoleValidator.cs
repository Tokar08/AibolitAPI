namespace AibolitAPI.Interfaces;

public interface IRoleValidator
{
    Task<bool> ValidateUserRoleAsync(HttpContext context, string requiredRole);
}