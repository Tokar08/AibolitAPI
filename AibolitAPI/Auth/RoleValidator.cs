using System.IdentityModel.Tokens.Jwt;
using AibolitAPI.Interfaces;

namespace AibolitAPI.Auth;

public class RoleValidator : IRoleValidator
{
    private readonly IUserService _userService;

    public RoleValidator(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<bool> ValidateUserRoleAsync(HttpContext context, string requiredRole)
    {
        var token = GetTokenFromHeader(context);
        if (string.IsNullOrEmpty(token)) return false;

        var keycloakId = GetClaimFromToken(token, "sub");
        if (string.IsNullOrEmpty(keycloakId)) return false;

        var user = await _userService.GetUserByKeycloakIdAsync(keycloakId);
        return user?.Role?.Title == requiredRole;
    }

    private static string GetTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        return (authHeader?.StartsWith("Bearer ") == true
            ? authHeader.Substring("Bearer ".Length).Trim()
            : null)!;
    }

    private static string GetClaimFromToken(string token, string claimType)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value!;
    }
}