using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Services;

public class PatientService
{
    private readonly ILogger<PatientService> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository, IMapper mapper, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PatientDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(page, size,
                patient => patient
                    .Include(p => p.MedicalRecord)
                    .Include(p => p.Doctors)
                    .Include(p => p.User)
                    .Include(p => p.LikedDoctors)
            );
            return _mapper.Map<IEnumerable<PatientDTO>>(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all patients.");
            throw;
        }
    }

    public async Task<PatientDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            return _mapper.Map<PatientDTO>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting patient with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(PatientDTO patientDto)
    {
        try
        {
            var patient = _mapper.Map<Patient>(patientDto);
            await _patientRepository.CreateAsync(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating patient.");
            throw;
        }
    }

    public async Task UpdateAsync(PatientDTO patientDto)
    {
        try
        {
            var patient = _mapper.Map<Patient>(patientDto);
            await _patientRepository.UpdateAsync(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating patient.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _patientRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting patient with ID: {id}");
            throw;
        }
    }
}