using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;


namespace AibolitAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AibolitDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(AibolitDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RegisterAsync(Patient patient)
        {
            ArgumentNullException.ThrowIfNull(patient);

            if (string.IsNullOrWhiteSpace(patient.Email))
                throw new ArgumentException("Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(patient.PhoneNumber))
                throw new ArgumentException("Phone number cannot be empty.");

            if (string.IsNullOrWhiteSpace(patient.PasswordHash))
                throw new ArgumentException("Password cannot be empty.");

            if (string.IsNullOrWhiteSpace(patient.Username)) 
                throw new ArgumentException("Username cannot be empty.");

            patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(patient.PasswordHash);

            try
            {
                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                throw new InvalidOperationException("An error occurred while saving the entity changes.", ex);
            }
        }


        public async Task<User> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            User? user;
            
            user = await _context.Patients.FindAsync(username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;
            
            user = await _context.Doctors.FindAsync(username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;
            
            user = await _context.Administrators.FindAsync(username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;
            
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
    }
}
