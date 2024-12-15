using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class AdministratorService : IAdministratorService
{
    private readonly IAdministratorRepository _administratorRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IHospitalRepository _hospitalRepository;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<AdministratorService> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientRepository;

    public AdministratorService(IAdministratorRepository administratorRepository, IDoctorRepository doctorRepository,
        IHospitalRepository hospitalRepository,
        IMapper mapper, ILogger<AdministratorService> logger, IKeycloakService keycloakService,
        IPatientRepository patientRepository)
    {
        _administratorRepository = administratorRepository;
        _doctorRepository = doctorRepository;
        _hospitalRepository = hospitalRepository;
        _mapper = mapper;
        _logger = logger;
        _keycloakService = keycloakService;
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<AdministratorDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var administrators = await _administratorRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<AdministratorDTO>>(administrators);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all administrators.");
            throw;
        }
    }

    public async Task<string> GetAllAdministratorWithSSOAsync(int page, int size)
    {
        var dbAdministrators = await _administratorRepository.GetAllAsync(page, size);
        var administratorsWithSSO = await _keycloakService.GetEntitiesWithSSOAsync<AdministratorDTO, Administrator>(
            dbAdministrators,
            admin => admin.User.KeycloakId.ToString()
        );

        await Task.WhenAll(administratorsWithSSO.Select(admin => UpdateDoctorsAndPatients(admin, page, size)));

        return _keycloakService.Serialize(administratorsWithSSO);
    }

    public async Task<string> GetByIdAsync(Guid id)
    {
        try
        {
            var dbAdministrator = await _administratorRepository.GetByIdAsync(id) ??
                                  throw new KeyNotFoundException($"Administrator with ID: {id} was not found.");
            var administratorWithSSO = await _keycloakService.GetEntityWithSSOAsync<AdministratorDTO, Administrator>(
                dbAdministrator,
                admin => admin.User.KeycloakId.ToString()
            );

            await UpdateDoctorsAndPatients(administratorWithSSO, 1, int.MaxValue);
            return _keycloakService.Serialize(administratorWithSSO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting administrator with ID: {id}");
            throw new Exception($"Error occurred while processing administrator with ID: {id}", ex);
        }
    }

    public async Task CreateAsync(AdministratorDTO administratorDto)
    {
        try
        {
            _logger.LogInformation("Начало создания администратора.");

            _logger.LogInformation("Попытка найти больницу с ID: {ManagedHospitalId}",
                administratorDto.ManagedHospitalId);
            var hospital = await _hospitalRepository.GetByIdAsync(administratorDto.ManagedHospitalId);
            if (hospital == null)
            {
                _logger.LogError("Больница с ID {ManagedHospitalId} не найдена.", administratorDto.ManagedHospitalId);
                throw new Exception("Hospital not found.");
            }

            _logger.LogInformation("Больница с ID: {ManagedHospitalId} найдена.", administratorDto.ManagedHospitalId);


            _logger.LogInformation("Маппинг AdministratorDTO в Administrator.");
            var administrator = _mapper.Map<Administrator>(administratorDto);
            _logger.LogInformation("Маппинг успешно выполнен. Администратор готов к сохранению.");


            _logger.LogInformation("Попытка сохранить администратора с ID: {AdministratorId}.", administrator.Id);
            await _administratorRepository.CreateAsync(administrator);
            _logger.LogInformation("Администратор с ID: {AdministratorId} успешно сохранен.", administrator.Id);


            _logger.LogInformation("Попытка добавить администратора в коллекцию больницы.");
            hospital.Administrators.Add(administrator);
            _logger.LogInformation("Администратор добавлен в коллекцию больницы.");


            _logger.LogInformation("Попытка обновить больницу с ID: {ManagedHospitalId}.",
                administratorDto.ManagedHospitalId);
            await _hospitalRepository.UpdateAsync(hospital);
            _logger.LogInformation("Больница с ID: {ManagedHospitalId} успешно обновлена.",
                administratorDto.ManagedHospitalId);

            _logger.LogInformation("Администратор успешно создан.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании администратора.");
            throw;
        }
    }


    public async Task UpdateAsync(AdministratorDTO administratorDto)
    {
        try
        {
            var administrator = _mapper.Map<Administrator>(administratorDto);
            await _administratorRepository.UpdateAsync(administrator);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating administrator.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _administratorRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting administrator with ID: {id}");
            throw;
        }
    }

    private async Task UpdateDoctorsAndPatients(AdministratorDTO administrator, int page, int size)
    {
        if (administrator.Doctors != null && administrator.Doctors.Count != 0)
            await UpdateDoctors(administrator.Doctors, page, size);

        if (administrator.Patients != null && administrator.Patients.Count != 0)
            await UpdatePatients(administrator.Patients, page, size);
    }

    private async Task UpdateDoctors(IEnumerable<DoctorDTO> doctors, int page, int size)
    {
        var doctorEntities = await _keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
            await _doctorRepository.GetAllAsync(page, size),
            doctor => doctor.User.KeycloakId.ToString()
        );

        foreach (var doctor in doctors)
        {
            var doctorEntity = doctorEntities.FirstOrDefault(d => d.Id == doctor.Id);
            if (doctorEntity == null) continue;

            doctor.Email = doctorEntity.Email;
            doctor.FirstName = doctorEntity.FirstName;
            doctor.LastName = doctorEntity.LastName;
            doctor.PhoneNumber = doctorEntity.PhoneNumber;
            doctor.Gender = doctorEntity.Gender;
            doctor.City = doctorEntity.City;
            doctor.BirthDate = doctorEntity.BirthDate;


            if (doctor.Patients != null && doctor.Patients.Count != 0)
                await UpdatePatients(doctor.Patients, page, size);
        }
    }

    private async Task UpdatePatients(IEnumerable<PatientDTO> patients, int page, int size)
    {
        var patientEntities = await _keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
            await _patientRepository.GetAllAsync(page, size),
            patient => patient.User.KeycloakId.ToString()
        );

        foreach (var patient in patients)
        {
            var patientEntity = patientEntities.FirstOrDefault(p => p.Id == patient.Id);
            if (patientEntity == null) continue;

            patient.Email = patientEntity.Email;
            patient.FirstName = patientEntity.FirstName;
            patient.LastName = patientEntity.LastName;
            patient.PhoneNumber = patientEntity.PhoneNumber;
            patient.Gender = patientEntity.Gender;
            patient.City = patientEntity.City;
            patient.BirthDate = patientEntity.BirthDate;
        }
    }
}