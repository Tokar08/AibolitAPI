using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class DoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ILogger<DoctorService> _logger;
    private readonly IMapper _mapper;

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

    public async Task<string> CreateWithPhotoAsync(DoctorDTO doctorDto, Stream? photoStream, string? fileName)
    {
        try
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);

            if (photoStream != null && !string.IsNullOrWhiteSpace(fileName))
            {
                var uploadedPhotoUrl = await _doctorRepository.UploadPhotoAsync(photoStream, fileName);
                doctor.PhotoUrl = uploadedPhotoUrl;
            }

            await _doctorRepository.CreateAsync(doctor);
            return doctor.PhotoUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating doctor.");
            throw;
        }
    }

    public async Task<string> UpdateWithPhotoAsync(Guid id, DoctorDTO doctorDto, Stream? photoStream, string? fileName)
    {
        try
        {
            var existingDoctor = await _doctorRepository.GetByIdAsync(id);
            if (existingDoctor == null)
                throw new KeyNotFoundException($"Doctor with ID {id} not found.");

            var oldPhotoUrl = existingDoctor.PhotoUrl;
            _mapper.Map(doctorDto, existingDoctor);

            if (photoStream != null && !string.IsNullOrWhiteSpace(fileName))
            {
                if (!oldPhotoUrl.Contains(fileName))
                {
                    if (!string.IsNullOrWhiteSpace(oldPhotoUrl))
                        await _doctorRepository.DeletePhotoAsync(oldPhotoUrl);

                    var uploadedPhotoUrl = await _doctorRepository.UploadPhotoAsync(photoStream, fileName);
                    existingDoctor.PhotoUrl = uploadedPhotoUrl;
                }
            }
            else
            {
                existingDoctor.PhotoUrl = oldPhotoUrl;
            }

            await _doctorRepository.UpdateAsync(existingDoctor);
            return existingDoctor.PhotoUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating doctor with ID: {id}");
            throw;
        }
    }


    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor != null && !string.IsNullOrWhiteSpace(doctor.PhotoUrl))
                await _doctorRepository.DeletePhotoAsync(doctor.PhotoUrl);

            await _doctorRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting doctor with ID: {id}");
            throw;
        }
    }
}