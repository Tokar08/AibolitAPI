using System;
using System.Threading.Tasks;
using AibolitAPI.Auth;
using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AibolitAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IMapper mapper, IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (registerDto == null)
                return BadRequest("Invalid registration data.");
            
            var patient = _mapper.Map<Patient>(registerDto);
            patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            try
            {
                await _userService.RegisterAsync(patient);
                var patientDto = _mapper.Map<PatientDTO>(patient);
                return Created("user", patientDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Registration failed: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (loginDto == null)
                return BadRequest("Invalid login data.");

            try
            {
                var user = await _userService.LoginAsync(loginDto.Username, loginDto.Password);
                
                var secretKey = _configuration["TokenKey"];
                var token = JwtGenerator.GenerateJwt(user, secretKey, DateTime.UtcNow.AddHours(1));

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized($"Login failed: {ex.Message}");
            }
        }
    }
}
