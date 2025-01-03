using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Interfaces.Services;
using AibolitAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Services;

public class HospitalService : IHospitalService
{
    private readonly IAdministratorRepository _administratorRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorService _doctorService;
    private readonly IFilterService _filterService;
    private readonly IHospitalRepository _hospitalRepository;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<HospitalService> _logger;
    private readonly IMapper _mapper;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IPatientRepository _patientRepository;

    public HospitalService(IHospitalRepository hospitalRepository, IMapper mapper, ILogger<HospitalService> logger,
        IAdministratorRepository administratorRepository, IDoctorRepository doctorRepository,
        IKeycloakService keycloakService, IPatientRepository patientRepository,
        IMedicalRecordRepository medicalRecordRepository, IDoctorService doctorService, IFilterService filterService)
    {
        _hospitalRepository = hospitalRepository;
        _mapper = mapper;
        _logger = logger;
        _administratorRepository = administratorRepository;
        _doctorRepository = doctorRepository;
        _keycloakService = keycloakService;
        _patientRepository = patientRepository;
        _medicalRecordRepository = medicalRecordRepository;
        _doctorService = doctorService;
        _filterService = filterService;
    }

    public async Task<string> GetAllHospitalsWithDetailsAsync(int page, int size)
    {
        var dbHospitals = await _hospitalRepository.GetAllAsync(page, size);
        var hospitalEntities = await _keycloakService.GetEntitiesWithSSOAsync<HospitalDTO, Hospital>(
            dbHospitals,
            hospital => hospital.Id.ToString()
        );

        foreach (var hospital in hospitalEntities) await UpdateDoctorsAndAdminsForHospital(hospital, page, size);

        return _keycloakService.Serialize(hospitalEntities);
    }

    public async Task<string> GetDoctorsByHospitalIdAsync(Guid hospitalId, DoctorFilterDTO? filterDto, int page,
        int size)
    {
        var dbDoctors = await _doctorRepository.GetAllAsync(page, size, q => q.Where(d => d.HospitalId == hospitalId));

        var doctorEntities = await _keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
            dbDoctors,
            doctor => doctor.User.KeycloakId.ToString()
        );

        var filteredDoctors = _filterService.ApplyDoctorFilter(doctorEntities, filterDto, dbDoctors);

        return _keycloakService.Serialize(filteredDoctors.Where(d => d.IsActive));
    }


    public async Task<string> GetDoctorWithPatientsAsync(Guid hospitalId, Guid doctorId, int page, int size)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.HospitalId != hospitalId)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} in Hospital with ID: {hospitalId} not found.");

        var doctorEntity =
            await _keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(doctor, d => d.User.KeycloakId.ToString());
        doctorEntity.Patients = await UpdatePatientsForDoctor(doctorEntity, page, size);

        return _keycloakService.Serialize(doctorEntity);
    }

    public async Task<string> GetPatientsForDoctorAsync(Guid hospitalId, Guid doctorId, int page, int size,
        PatientFilterDTO? filterDto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.HospitalId != hospitalId)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} in Hospital with ID: {hospitalId} not found.");

        var patients = doctor.Patients
            .Where(p => p.Doctors.Any(d => d.Id == doctorId))
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        var patientEntities = await _keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
            patients,
            patient => patient.User.KeycloakId.ToString()
        );

        var filteredPatients = _filterService.ApplyPatientFilter(patientEntities, filterDto);
        return _keycloakService.Serialize(filteredPatients.ToList());
    }


    public async Task<string> GetPatientInfoAsync(Guid hospitalId, Guid doctorId, Guid patientId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.HospitalId != hospitalId)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} in Hospital with ID: {hospitalId} not found.");

        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null || patient.Doctors.All(d => d.Id != doctorId))
            throw new KeyNotFoundException(
                $"Patient with ID: {patientId} is not assigned to Doctor with ID: {doctorId}.");

        var patientEntity =
            await _keycloakService.GetEntityWithSSOAsync<PatientDTO, Patient>(patient,
                p => p.User.KeycloakId.ToString());
        return _keycloakService.Serialize(patientEntity);
    }

    public async Task<IEnumerable<PrescriptionDTO>> GetPatientPrescriptionsAsync(
        Guid hospitalId,
        Guid doctorId,
        Guid patientId,
        PrescriptionFilterDTO? filterDto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.HospitalId != hospitalId)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} in Hospital with ID: {hospitalId} not found.");

        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null || patient.Doctors.All(d => d.Id != doctorId))
            throw new KeyNotFoundException(
                $"Patient with ID: {patientId} is not assigned to Doctor with ID: {doctorId}.");

        var medicalRecord = await _medicalRecordRepository.GetByPatientIdAsync(patientId);
        if (medicalRecord == null)
            throw new KeyNotFoundException($"Medical record for Patient with ID: {patientId} not found.");

        var prescriptions = medicalRecord.Prescriptions.Where(p => p.IsActive).ToList();
        var prescriptionDTOs = _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);

        var filteredPrescriptions = _filterService.ApplyPrescriptionFilter(prescriptionDTOs, filterDto);

        return filteredPrescriptions;
    }


    public async Task<IEnumerable<RecommendationDTO>> GetPatientRecommendationsAsync(
        Guid hospitalId,
        Guid doctorId,
        Guid patientId,
        RecommendationFilterDTO? filterDto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.HospitalId != hospitalId)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} in Hospital with ID: {hospitalId} not found.");

        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null || patient.Doctors.All(d => d.Id != doctorId))
            throw new KeyNotFoundException(
                $"Patient with ID: {patientId} is not assigned to Doctor with ID: {doctorId}.");

        var medicalRecord = await _medicalRecordRepository.GetByPatientIdAsync(patientId);
        if (medicalRecord == null)
            throw new KeyNotFoundException($"Medical record for Patient with ID: {patientId} not found.");

        var recommendations = medicalRecord.Recommendations.Where(r => r.IsActive).ToList();
        var recommendationDTOs = _mapper.Map<IEnumerable<RecommendationDTO>>(recommendations);

        var filteredRecommendations = _filterService.ApplyRecommendationFilter(recommendationDTOs, filterDto);

        return filteredRecommendations;
    }


    public async Task<object> GetPatientRecommendationByIdAsync(Guid hospitalId, Guid doctorId,
        Guid patientId, Guid recommendationId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.HospitalId != hospitalId)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} in Hospital with ID: {hospitalId} not found.");

        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null || patient.Doctors.All(d => d.Id != doctorId))
            throw new KeyNotFoundException(
                $"Patient with ID: {patientId} is not assigned to Doctor with ID: {doctorId}.");

        var medicalRecord = await _medicalRecordRepository.GetByPatientIdAsync(patientId);
        if (medicalRecord == null)
            throw new KeyNotFoundException($"Medical record for Patient with ID: {patientId} not found.");

        var recommendation = medicalRecord.Recommendations.FirstOrDefault(r => r.Id == recommendationId && r.IsActive);
        if (recommendation == null)
            throw new KeyNotFoundException($"Recommendation with ID: {recommendationId} not found.");

        var doctorDto = await _keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
            doctor,
            d => d.User.KeycloakId.ToString()
        );

        return new
        {
            RecommendationId = recommendation.Id,
            recommendation.PatientId,
            recommendation.DoctorId,
            recommendation.MedicalRecordId,
            recommendation.Content,
            recommendation.RecommendationDate,
            recommendation.IsActive,
            DoctorName = $"{doctorDto.FirstName} {doctorDto.LastName}"
        };
    }

    public async Task<object> GetPatientPrescriptionByIdAsync(Guid hospitalId, Guid doctorId,
        Guid patientId, Guid prescriptionId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.HospitalId != hospitalId)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} in Hospital with ID: {hospitalId} not found.");

        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null || patient.Doctors.All(d => d.Id != doctorId))
            throw new KeyNotFoundException(
                $"Patient with ID: {patientId} is not assigned to Doctor with ID: {doctorId}.");

        var medicalRecord = await _medicalRecordRepository.GetByPatientIdAsync(patientId);
        if (medicalRecord == null)
            throw new KeyNotFoundException($"Medical record for Patient with ID: {patientId} not found.");

        var prescription = medicalRecord.Prescriptions.FirstOrDefault(p => p.Id == prescriptionId && p.IsActive);
        if (prescription == null)
            throw new KeyNotFoundException($"Prescription with ID: {prescriptionId} not found.");

        var doctorDto = await _keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
            doctor,
            d => d.User.KeycloakId.ToString()
        );

        return new
        {
            PrescriptionId = prescription.Id,
            prescription.PatientId,
            prescription.DoctorId,
            prescription.MedicalRecordId,
            prescription.MedicationName,
            prescription.Dosage,
            prescription.Instructions,
            prescription.PrescriptionDate,
            prescription.IsActive,
            DoctorName = $"{doctorDto.FirstName} {doctorDto.LastName}"
        };
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

    private async Task UpdateDoctorsAndAdminsForHospital(HospitalDTO hospital, int page, int size)
    {
        if (hospital.Doctors?.Count > 0)
        {
            var dbDoctors = await _doctorRepository.GetAllAsync(page, size);
            var doctorEntities = (await _keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
                dbDoctors,
                doctor => doctor.User.KeycloakId.ToString()
            )).ToList();

            foreach (var doctor in hospital.Doctors) await UpdateDoctorDetails(doctor, doctorEntities, page, size);
        }

        if (hospital.Administrators?.Count > 0)
        {
            var dbAdministrators = await _administratorRepository.GetAllAsync(page, size);
            var adminEntities = (await _keycloakService.GetEntitiesWithSSOAsync<AdministratorDTO, Administrator>(
                dbAdministrators,
                admin => admin.User.KeycloakId.ToString()
            )).ToList();

            foreach (var admin in hospital.Administrators) await UpdateAdminDetails(admin, adminEntities);
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
}