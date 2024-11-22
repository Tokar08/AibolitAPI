using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class DoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IDoctorRepository doctorRepository, IMapper mapper, ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<DoctorDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var doctors = await _doctorRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<DoctorDTO>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all doctors.");
            throw;
        }
    }

    public async Task<DoctorDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            return _mapper.Map<DoctorDTO>(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting doctor with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(DoctorDTO doctorDto)
    {
        try
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);
            await _doctorRepository.CreateAsync(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating doctor.");
            throw;
        }
    }

    public async Task UpdateAsync(DoctorDTO doctorDto)
    {
        try
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);
            await _doctorRepository.UpdateAsync(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating doctor.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _doctorRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting doctor with ID: {id}");
            throw;
        }
    }
}