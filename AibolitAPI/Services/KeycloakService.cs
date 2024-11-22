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

            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var gender = jwtToken.Claims.FirstOrDefault(c => c.Type == "gender")?.Value;
            var address = jwtToken.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
            var phoneNumber = jwtToken.Claims.FirstOrDefault(c => c.Type == "phoneNumber")?.Value;
            var birthDateString = jwtToken.Claims.FirstOrDefault(c => c.Type == "birthDate")?.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Insufficient data in token.");
                return;
            }

            if (!DateTime.TryParse(birthDateString, out var birthDate))
            {
                _logger.LogWarning("Invalid birthDate format in token. Defaulting to DateTime.MinValue.");
                birthDate = DateTime.MinValue;
            }

            await _userService.AuthenticateOrRegisterAsync(username, email, name, gender, address, phoneNumber,
                birthDate);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error processing token: {Message}", ex.Message);
        }
    }
}