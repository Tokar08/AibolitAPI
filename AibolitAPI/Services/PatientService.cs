using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Services;

public class PatientService : IPatientService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<PatientService> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository, IMapper mapper, ILogger<PatientService> logger,
        IKeycloakService keycloakService, IDoctorRepository doctorRepository)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
        _logger = logger;
        _keycloakService = keycloakService;
        _doctorRepository = doctorRepository;
    }


    public async Task<string> GetAllPatientsWithSSOAsync(int page, int size)
    {
        var dbPatients = await _patientRepository.GetAllAsync(page, size);
        var patientEntities = await _keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
            dbPatients,
            patient => patient.User.KeycloakId.ToString()
        );

        foreach (var patient in patientEntities) await UpdateDoctorsForPatient(patient, page, size);

        return _keycloakService.Serialize(patientEntities);
    }


    public async Task<IEnumerable<PatientDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(page, size,
                patient => patient
                    .Include(p => p.Doctors)
                    .Include(p => p.LikedDoctors)
                    .Include(p => p.User)
            );

            var patientDTOs = _mapper.Map<IEnumerable<PatientDTO>>(patients);

            return patientDTOs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all patients.");
            throw;
        }
    }

    public async Task<string> GetByIdAsync(Guid id)
    {
        try
        {
            var dbPatient = await _patientRepository.GetByIdAsync(id);
            if (dbPatient == null)
            {
                _logger.LogWarning($"Patient with ID: {id} was not found.");
                throw new KeyNotFoundException($"Patient with ID: {id} was not found.");
            }

            var patientWithSSO = await _keycloakService.GetEntityWithSSOAsync<PatientDTO, Patient>(
                dbPatient,
                patient => patient.User.KeycloakId.ToString()
            );

            await UpdateDoctorsForPatient(patientWithSSO, 1, int.MaxValue);

            return _keycloakService.Serialize(patientWithSSO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting patient with ID: {id}");
            throw new Exception($"Error occurred while processing patient with ID: {id}", ex);
        }
    }

    private async Task UpdateDoctorsForPatient(PatientDTO patient, int page, int size)
    {
        if (patient.Doctors != null && patient.Doctors.Count > 0)
        {
            var dbDoctors = await _doctorRepository.GetAllAsync(page, size);
            var doctorEntities = await _keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
                dbDoctors,
                doctor => doctor.User.KeycloakId.ToString()
            );

            var doctorDTOs = doctorEntities.ToList();

            for (var i = 0; i < patient.Doctors.Count; i++)
            {
                var doctorDTO = patient.Doctors.ElementAt(i);
                var doctorEntity = doctorDTOs.ElementAtOrDefault(i);

                if (doctorEntity == null) continue;

                doctorDTO.Email = doctorEntity.Email ?? string.Empty;
                doctorDTO.FirstName = doctorEntity.FirstName ?? string.Empty;
                doctorDTO.LastName = doctorEntity.LastName ?? string.Empty;
                doctorDTO.PhoneNumber = doctorEntity.PhoneNumber ?? string.Empty;
                doctorDTO.Gender = doctorEntity.Gender ?? string.Empty;
                doctorDTO.City = doctorEntity.City ?? string.Empty;
                doctorDTO.BirthDate = doctorEntity.BirthDate ?? DateTime.MinValue.ToString();
            }
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