using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class UserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDTO> AuthenticateOrRegisterAsync(string keycloakId)
    {
        try
        {
            var existingUser = (await _userRepository.GetAllAsync(1, int.MaxValue))
                .FirstOrDefault(u => u.KeycloakId == keycloakId);

            if (existingUser != null)
                return _mapper.Map<UserDTO>(existingUser);

            var defaultRole = await _userRepository.GetRoleByNameAsync("Patient")
                              ?? throw new Exception("Default role 'Patient' not found in database");

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
            return _mapper.Map<UserDTO>(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while authenticating or registering user.");
            throw;
        }
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var users = await _userRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users.");
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