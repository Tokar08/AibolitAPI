using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<DoctorService> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientRepository;

    public DoctorService(IDoctorRepository doctorRepository, IPatientRepository patientRepository, IMapper mapper,
        ILogger<DoctorService> logger,
        IKeycloakService keycloakService)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _mapper = mapper;
        _logger = logger;
        _keycloakService = keycloakService;
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

    public async Task<string> GetAllDoctorsWithSSOAsync(int page, int size)
    {
        var dbDoctors = await _doctorRepository.GetAllAsync(page, size);
        var doctorEntities = await _keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
            dbDoctors,
            doctor => doctor.User.KeycloakId.ToString()
        );

        await Task.WhenAll(doctorEntities.Select(doctor => UpdatePatientsForDoctor(doctor, page, size)));

        return _keycloakService.Serialize(doctorEntities);
    }

    public async Task<string> GetByIdAsync(Guid id)
    {
        try
        {
            var dbDoctor = await _doctorRepository.GetByIdAsync(id);
            if (dbDoctor == null)
            {
                _logger.LogWarning($"Doctor with ID: {id} was not found.");
                throw new KeyNotFoundException($"Doctor with ID: {id} was not found.");
            }

            var doctorWithSSO = await _keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
                dbDoctor,
                doctor => doctor.User.KeycloakId.ToString()
            );

            await UpdatePatientsForDoctor(doctorWithSSO, 1, int.MaxValue);

            return _keycloakService.Serialize(doctorWithSSO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting doctor with ID: {id}");
            throw new Exception($"Error occurred while processing doctor with ID: {id}", ex);
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

    private async Task UpdatePatientsForDoctor(DoctorDTO doctor, int page, int size)
    {
        if (doctor.Patients != null && doctor.Patients.Count > 0)
        {
            var dbPatients = await _patientRepository.GetAllAsync(page, size);
            var patientEntities = await _keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
                dbPatients,
                patient => patient.User.KeycloakId.ToString()
            );

            var patientDTOs = patientEntities.ToList();

            for (var i = 0; i < doctor.Patients.Count; i++)
            {
                var patientDTO = doctor.Patients.ElementAt(i);
                var patientEntity = patientDTOs.ElementAtOrDefault(i);

                if (patientEntity == null) continue;

                patientDTO.Email = patientEntity.Email ?? string.Empty;
                patientDTO.FirstName = patientEntity.FirstName ?? string.Empty;
                patientDTO.LastName = patientEntity.LastName ?? string.Empty;
                patientDTO.PhoneNumber = patientEntity.PhoneNumber ?? string.Empty;
                patientDTO.Gender = patientEntity.Gender ?? string.Empty;
                patientDTO.City = patientEntity.City ?? string.Empty;
                patientDTO.BirthDate = patientEntity.BirthDate ?? DateTime.MinValue.ToString();
            }
        }
    }
}