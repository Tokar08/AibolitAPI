using System.Net.Http.Headers;
using AibolitAPI.Data;
using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AibolitAPI.Services;

public class UserService
{
    private readonly AibolitDbContext _dbContext;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationSender _notificationSender;
    private readonly IEmailTemplateFactory _templateFactory;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger,
        AibolitDbContext dbContext, INotificationSender notificationSender, IEmailTemplateFactory templateFactory)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _dbContext = dbContext;
        _notificationSender = notificationSender;
        _templateFactory = templateFactory;
    }

    public async Task<UserDTO> AuthenticateOrRegisterAsync(string keycloakId, string email, string userName,
        string birthDateClaim)
    {
        try
        {
            var existingUser = (await _userRepository.GetAllAsync(1, int.MaxValue))
                .FirstOrDefault(u => u.KeycloakId == keycloakId);

            if (existingUser != null)
                return _mapper.Map<UserDTO>(existingUser);

            var defaultRole = await _userRepository.GetRoleByNameAsync("Patient")
                              ?? throw new Exception("Default role 'Patient' not found");

            if (!DateTime.TryParse(birthDateClaim, out var birthDate) || birthDate.Date > DateTime.UtcNow.Date)
                throw new ArgumentException(
                    "Некоректна дата народження або дата народження не може бути у майбутньому.");


            var newUser = new User
            {
                Id = Guid.NewGuid(),
                KeycloakId = keycloakId,
                RoleId = defaultRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(newUser);

            var medicalRecord = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = newUser.Id,
                RecordDate = DateTime.UtcNow,
                IsActive = true,
                Appointments = new List<Appointment>(),
                Prescriptions = new List<Prescription>(),
                Recommendations = new List<Recommendation>()
            };

            _dbContext.MedicalRecords.Add(medicalRecord);
            await _dbContext.SaveChangesAsync();

            var newPatient = new Patient
            {
                Id = Guid.NewGuid(),
                UserId = newUser.Id,
                MedicalRecordId = medicalRecord.Id,
                IsActive = true
            };

            _dbContext.Patients.Add(newPatient);
            await _dbContext.SaveChangesAsync();

            var template = _templateFactory.GetTemplate("registration");
            await _notificationSender.SendAsync(email, template, userName);


            return _mapper.Map<UserDTO>(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while authenticating or registering user.");
            throw;
        }
    }


    public async Task<User> GetUserByKeycloakIdAsync(string keycloakId)
    {
        return await _userRepository.GetUserByKeycloakId(keycloakId);
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var users = await _userRepository.GetAllWithRolesAsync(page, size);
            var sortedUsers = users.OrderBy(u => u.Role.Title).ToList();

            foreach (var user in sortedUsers) Console.WriteLine($"User: {user.KeycloakId}, Role: {user.Role.Title}");

            return _mapper.Map<IEnumerable<UserDTO>>(sortedUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users.");
            throw;
        }
    }

    private async Task<string> GetAdminTokenAsync()
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

    public async Task<IEnumerable<Dictionary<string, object>>> GetAllUsersFromKeycloakAsync()
    {
        try
        {
            var adminToken = await GetAdminTokenAsync();

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await httpClient.GetAsync("http://localhost:8081/admin/realms/aibolit-api/users");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to retrieve users from Keycloak. Status code: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            var keycloakUsers = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

            if (keycloakUsers == null || !keycloakUsers.Any())
                throw new Exception("No users found in Keycloak.");

            foreach (var user in keycloakUsers)
                if (user.TryGetValue("attributes", out var attributes) && attributes is JObject attributesObj)
                    user["attributes"] = attributesObj.ToObject<Dictionary<string, List<string>>>();

            return keycloakUsers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving users from Keycloak.");
            throw;
        }
    }


    public async Task SynchronizeUsersWithKeycloak(string userToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userToken))
                throw new ArgumentException("Токен пользователя отсутствует или некорректен.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            var response = await httpClient.GetAsync("http://localhost:8081/admin/realms/aibolit-api/users");
            if (!response.IsSuccessStatusCode)
                throw new Exception(
                    $"Не удалось получить пользователей из Keycloak. Код ошибки: {response.StatusCode}");

            var keycloakUsersJson = await response.Content.ReadAsStringAsync();
            var keycloakUsers = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(keycloakUsersJson);

            if (keycloakUsers == null || !keycloakUsers.Any())
                throw new Exception("Список пользователей из Keycloak пуст.");

            // Извлекаем KeycloakId из ответа
            var keycloakIds = keycloakUsers
                .Where(u => u.ContainsKey("id"))
                .Select(u => u["id"]?.ToString())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet();

            // Получаем пользователей из БД
            var dbUsers = await _userRepository.GetAllAsync(1, int.MaxValue);

            // Удаляем пользователей, которых нет в Keycloak
            var usersToDelete = dbUsers.Where(dbUser => !keycloakIds.Contains(dbUser.KeycloakId)).ToList();
            foreach (var user in usersToDelete)
            {
                await _userRepository.SoftDeleteAsync(user.Id);
                Console.WriteLine($"Удалён пользователь из БД: {user.KeycloakId}");
            }

            Console.WriteLine("Синхронизация завершена.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка во время синхронизации с Keycloak.");
            throw;
        }
    }


    public async Task<UserDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserDTO>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting user with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(UserDTO userDto)
    {
        try
        {
            var user = _mapper.Map<User>(userDto);
            await _userRepository.CreateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user.");
            throw;
        }
    }

    public async Task UpdateAsync(UserDTO userDto)
    {
        try
        {
            var user = _mapper.Map<User>(userDto);
            await _userRepository.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _userRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting user with ID: {id}");
            throw;
        }
    }
}