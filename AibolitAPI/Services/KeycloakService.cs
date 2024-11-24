using System.IdentityModel.Tokens.Jwt;

namespace AibolitAPI.Services;

public class KeycloakService
{
    private readonly ILogger<KeycloakService> _logger;
    private readonly UserService _userService;

    public KeycloakService(UserService userService, ILogger<KeycloakService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task ProcessTokenAsync(string token)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var keycloakId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(keycloakId))
            {
                _logger.LogWarning("Token does not contain a 'sub' claim (Keycloak ID).");
                return;
            }

            var user = await _userService.AuthenticateOrRegisterAsync(keycloakId);

            _logger.LogInformation("User authenticated or registered: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing token.");
            throw;
        }
    }
}