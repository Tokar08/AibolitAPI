using System.IdentityModel.Tokens.Jwt;
using AibolitAPI.Interfaces;
using AutoMapper;
using Newtonsoft.Json;

namespace AibolitAPI.Services;

public class KeycloakService : IKeycloakService
{
    private readonly ILogger<KeycloakService> _logger;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public KeycloakService(IServiceProvider serviceProvider, ILogger<KeycloakService> logger, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessTokenAsync(string token)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var keycloakId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var birthDate = jwtToken.Claims.FirstOrDefault(c => c.Type == "birthDate")?.Value;
            if (string.IsNullOrEmpty(keycloakId))
            {
                _logger.LogWarning("Token does not contain a 'sub' claim (Keycloak ID).");
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Token does not contain a 'email' claim (Email).");
                return;
            }

            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogWarning("Token does not contain a 'name' claim (User Name).");
                return;
            }

            if (string.IsNullOrEmpty(birthDate))
            {
                _logger.LogWarning("Token does not contain a 'birthDate' claim (Birth Date).");
                return;
            }

            var _userService = _serviceProvider.GetRequiredService<IUserService>();
            var user = await _userService.AuthenticateOrRegisterAsync(keycloakId, email, userName, birthDate);

            _logger.LogInformation("User authenticated or registered: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing token.");
            throw;
        }
    }

    public async Task<IEnumerable<TDto>> GetEntitiesWithSSOAsync<TDto, TEntity>(IEnumerable<TEntity> entities,
        Func<TEntity, string> keycloakIdSelector) where TEntity : class
    {
        try
        {
            var _userService = _serviceProvider.GetRequiredService<IUserService>();
            var keycloakUsers = await _userService.GetAllUsersFromKeycloakAsync();
            var keycloakUsersDict = keycloakUsers
                .Where(u => u.ContainsKey("id"))
                .ToDictionary(u => u["id"]?.ToString(), u => u);

            var entitiesWithSSO = entities.Select<TEntity, TDto>(entity =>
            {
                var entityDto = _mapper.Map<TDto>(entity);
                var userKeycloakId = keycloakIdSelector(entity);

                if (string.IsNullOrEmpty(userKeycloakId) ||
                    !keycloakUsersDict.TryGetValue(userKeycloakId, out var ssoData))
                    return entityDto;

                entityDto = MapSSODataToDto(entityDto, ssoData);

                return entityDto;
            }).ToList();

            return entitiesWithSSO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing entities with SSO data.");
            throw;
        }
    }

    public async Task<TDto> GetEntityWithSSOAsync<TDto, TEntity>(TEntity entity,
        Func<TEntity, string> keycloakIdSelector)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entitiesWithSSO = await GetEntitiesWithSSOAsync<TDto, TEntity>(new[] { entity }, keycloakIdSelector);
        return entitiesWithSSO.FirstOrDefault();
    }


    private TDto MapSSODataToDto<TDto>(TDto dto, Dictionary<string, object> ssoData)
    {
        var commonMapping = new Dictionary<string, Action<TDto, string>>
        {
            { "email", (d, value) => SetProperty(d, "Email", value) },
            { "firstName", (d, value) => SetProperty(d, "FirstName", value) },
            { "lastName", (d, value) => SetProperty(d, "LastName", value) },
            { "phoneNumber", (d, value) => SetProperty(d, "PhoneNumber", value) },
            { "gender", (d, value) => SetProperty(d, "Gender", value) },
            { "city", (d, value) => SetProperty(d, "City", value) },
            { "birthDate", (d, value) => SetProperty(d, "BirthDate", value) }
        };

        foreach (var (key, setter) in commonMapping)
        {
            var value = key == "phoneNumber" || key == "gender" || key == "city" || key == "birthDate"
                ? GetKeycloakAttribute(ssoData, key)
                : ssoData.GetValueOrDefault(key)?.ToString();

            if (value != null)
                setter(dto, value);
        }

        return dto;
    }

    private void SetProperty<TDto>(TDto dto, string propertyName, string value)
    {
        var property = typeof(TDto).GetProperty(propertyName);
        if (property != null && property.CanWrite)
            property.SetValue(dto, Convert.ChangeType(value, property.PropertyType));
    }


    private string GetKeycloakAttribute(Dictionary<string, object> ssoData, string attributeKey)
    {
        if (ssoData.GetValueOrDefault("attributes") is Dictionary<string, List<string>> attributes &&
            attributes.TryGetValue(attributeKey, out var values))
            return values.FirstOrDefault();
        return null;
    }

    public async Task<string> GetAdminTokenAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post,
                "http://localhost:8081/realms/master/protocol/openid-connect/token");

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", "admin-cli" },
                { "username", "admin" },
                { "password", "admin" },
                { "grant_type", "password" }
            });

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to retrieve admin token. Status code: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

            if (tokenResponse == null || !tokenResponse.ContainsKey("access_token"))
                throw new Exception("Invalid token response from Keycloak.");

            return tokenResponse["access_token"];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving admin token from Keycloak.");
            throw;
        }
    }

    public string Serialize(object obj, int maxDepth = 2,
        ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting formatting = Formatting.Indented)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            MaxDepth = maxDepth,
            ReferenceLoopHandling = referenceLoopHandling,
            Formatting = formatting
        });
    }
}