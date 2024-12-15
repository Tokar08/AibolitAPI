using System.Net.Http.Headers;
using AibolitAPI.Data;
using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AibolitAPI.Services;

public class UserService : IUserService
{
    private readonly AibolitDbContext _dbContext;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationSender _notificationSender;
    private readonly IEmailTemplateFactory _templateFactory;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger,
        AibolitDbContext dbContext, INotificationSender notificationSender, IEmailTemplateFactory templateFactory,
        IKeycloakService keycloakService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _dbContext = dbContext;
        _notificationSender = notificationSender;
        _templateFactory = templateFactory;
        _keycloakService = keycloakService;
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
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users.");
            throw;
        }
    }


    public async Task<IEnumerable<Dictionary<string, object>>> GetAllUsersFromKeycloakAsync()
    {
        try
        {
            var adminToken = await _keycloakService.GetAdminTokenAsync();

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

    public async Task<IEnumerable<Dictionary<string, object>>> GetExistingUsersAsync()
    {
        try
        {
            var keycloakUsers = await GetAllUsersFromKeycloakAsync();

            var dbUserKeycloakIds = _dbContext.Users
                .Select(u => u.KeycloakId)
                .ToHashSet();

            var existingUsers = keycloakUsers
                .Where(kcUser => kcUser.TryGetValue("id", out var id) && dbUserKeycloakIds.Contains(id?.ToString()))
                .ToList();

            return existingUsers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving existing users.");
            throw;
        }
    }


    public async Task<IEnumerable<UserDTO>> GetUsersWithSSOAsync(int page, int size)
    {
        try
        {
            var dbUsers = await _userRepository.GetAllWithRolesAsync(page, size);
            return await _keycloakService.GetEntitiesWithSSOAsync<UserDTO, User>(
                dbUsers,
                user => user.KeycloakId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting users with SSO data.");
            throw;
        }
    }


    public async Task<UserDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning($"User with ID: {id} was not found.");
                throw new KeyNotFoundException($"User with ID: {id} was not found.");
            }

            var userWithSSO = await _keycloakService.GetEntityWithSSOAsync<UserDTO, User>(
                user,
                u => u.KeycloakId.ToString()
            );

            return userWithSSO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting user with ID: {id}");
            throw new Exception($"Error occurred while processing user with ID: {id}", ex);
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