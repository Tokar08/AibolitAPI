using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Services;

public class HospitalService : IHospitalService
{
    private readonly IAdministratorRepository _administratorRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IHospitalRepository _hospitalRepository;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<HospitalService> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientRepository;

    public HospitalService(IHospitalRepository hospitalRepository, IMapper mapper, ILogger<HospitalService> logger,
        IAdministratorRepository administratorRepository, IDoctorRepository doctorRepository,
        IKeycloakService keycloakService, IPatientRepository patientRepository)
    {
        _hospitalRepository = hospitalRepository;
        _mapper = mapper;
        _logger = logger;
        _administratorRepository = administratorRepository;
        _doctorRepository = doctorRepository;
        _keycloakService = keycloakService;
        _patientRepository = patientRepository;
    }

    public async Task<string> GetAllHospitalsWithDetailsAsync(int page, int size)
    {
        var dbHospitals = await _hospitalRepository.GetAllAsync(page, size);
        var hospitalEntities = await _keycloakService.GetEntitiesWithSSOAsync<HospitalDTO, Hospital>(
            dbHospitals,
            hospital => hospital.Id.ToString()
        );

        await Task.WhenAll(hospitalEntities.Select(hospital =>
            UpdateDoctorsAndAdminsForHospital(hospital, page, size)));

        return _keycloakService.Serialize(hospitalEntities);
    }

    public async Task<IEnumerable<HospitalDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var hospitals = await _hospitalRepository.GetAllAsync(page, size,
                q => q
                    .Include(h => h.Administrators)
                    .Include(h => h.Doctors));

            return _mapper.Map<IEnumerable<HospitalDTO>>(hospitals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all hospitals.");
            throw;
        }
    }

    public async Task<string> GetByIdAsync(Guid id)
    {
        try
        {
            var dbHospital = await _hospitalRepository.GetByIdAsync(id);
            if (dbHospital == null)
            {
                _logger.LogWarning($"Hospital with ID: {id} was not found.");
                throw new KeyNotFoundException($"Hospital with ID: {id} was not found.");
            }

            var hospitalWithSSO = await _keycloakService.GetEntityWithSSOAsync<HospitalDTO, Hospital>(
                dbHospital,
                hospital => hospital.Id.ToString()
            );

            await UpdateDoctorsAndAdminsForHospital(hospitalWithSSO, 1, int.MaxValue);

            return _keycloakService.Serialize(hospitalWithSSO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting hospital with ID: {id}");
            throw new Exception($"Error occurred while processing hospital with ID: {id}", ex);
        }
    }

    private async Task UpdateDoctorsAndAdminsForHospital(HospitalDTO hospital, int page, int size)
    {
        if (hospital.Doctors?.Count > 0)
        {
            var dbDoctors = await _doctorRepository.GetAllAsync(page, size);
            var doctorEntities = (await _keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
                dbDoctors,
                doctor => doctor.User.KeycloakId.ToString()
            )).ToList();

            await Task.WhenAll(
                hospital.Doctors.Select(doctor => UpdateDoctorDetails(doctor, doctorEntities, page, size)));
        }

        if (hospital.Administrators?.Count > 0)
        {
            var dbAdministrators = await _administratorRepository.GetAllAsync(page, size);
            var adminEntities = (await _keycloakService.GetEntitiesWithSSOAsync<AdministratorDTO, Administrator>(
                dbAdministrators,
                admin => admin.User.KeycloakId.ToString()
            )).ToList();

            await Task.WhenAll(hospital.Administrators.Select(admin => UpdateAdminDetails(admin, adminEntities)));
        }
    }

    private async Task UpdateDoctorDetails(DoctorDTO doctor, List<DoctorDTO> doctorEntities, int page, int size)
    {
        var doctorEntity = doctorEntities.FirstOrDefault(d => d.Id == doctor.Id);
        if (doctorEntity == null) return;

        doctor.Email = doctorEntity.Email ?? string.Empty;
        doctor.FirstName = doctorEntity.FirstName ?? string.Empty;
        doctor.LastName = doctorEntity.LastName ?? string.Empty;
        doctor.PhoneNumber = doctorEntity.PhoneNumber ?? string.Empty;
        doctor.Gender = doctorEntity.Gender ?? string.Empty;
        doctor.City = doctorEntity.City ?? string.Empty;
        doctor.BirthDate = doctorEntity.BirthDate ?? DateTime.MinValue.ToString();
        doctor.Patients = await UpdatePatientsForDoctor(doctor, page, size);
    }

    private async Task UpdateAdminDetails(AdministratorDTO admin, List<AdministratorDTO> adminEntities)
    {
        var adminEntity = adminEntities.FirstOrDefault(a => a.Id == admin.Id);
        if (adminEntity == null) return;

        admin.Email = adminEntity.Email ?? string.Empty;
        admin.FirstName = adminEntity.FirstName ?? string.Empty;
        admin.LastName = adminEntity.LastName ?? string.Empty;
        admin.PhoneNumber = adminEntity.PhoneNumber ?? string.Empty;
        admin.Gender = adminEntity.Gender ?? string.Empty;
        admin.City = adminEntity.City ?? string.Empty;
        admin.BirthDate = adminEntity.BirthDate ?? DateTime.MinValue.ToString();
    }

    private async Task<List<PatientDTO>> UpdatePatientsForDoctor(DoctorDTO doctor, int page, int size)
    {
        if (doctor.Patients == null || doctor.Patients.Count == 0) return new List<PatientDTO>();

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

        return patientDTOs;
    }

    public async Task CreateAsync(HospitalDTO hospitalDto)
    {
        try
        {
            var hospital = _mapper.Map<Hospital>(hospitalDto);
            await _hospitalRepository.CreateAsync(hospital);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating hospital.");
            throw;
        }
    }

    public async Task UpdateAsync(HospitalDTO hospitalDto)
    {
        try
        {
            var hospital = _mapper.Map<Hospital>(hospitalDto);
            await _hospitalRepository.UpdateAsync(hospital);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating hospital.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _hospitalRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting hospital with ID: {id}");
            throw;
        }
    }
}