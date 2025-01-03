using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class AdministratorService(
    IAdministratorRepository administratorRepository,
    IDoctorRepository doctorRepository,
    IHospitalRepository hospitalRepository,
    IMapper mapper,
    ILogger<AdministratorService> logger,
    IKeycloakService keycloakService,
    IPatientRepository patientRepository,
    IUserRepository userRepository)
    : IAdministratorService
{
  
    public async Task<IEnumerable<AdministratorDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var administrators = await administratorRepository.GetAllAsync(page, size);
            return mapper.Map<IEnumerable<AdministratorDTO>>(administrators);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all administrators.");
            throw;
        }
    }

    public async Task<string> GetAllAdministratorWithSSOAsync(int page, int size)
    {
        var dbAdministrators = await administratorRepository.GetAllAsync(page, size);
        var administratorsWithSSO = await keycloakService.GetEntitiesWithSSOAsync<AdministratorDTO, Administrator>(
            dbAdministrators,
            admin => admin.User.KeycloakId.ToString()
        );

        //await Task.WhenAll(administratorsWithSSO.Select(admin => UpdateDoctorsAndPatients(admin, page, size)));

        return keycloakService.Serialize(administratorsWithSSO);
    }

    public async Task<string> GetByIdAsync(Guid id)
    {
        try
        {
            var dbAdministrator = await administratorRepository.GetByIdAsync(id) ??
                                  throw new KeyNotFoundException($"Administrator with ID: {id} was not found.");
            var administratorWithSSO = await keycloakService.GetEntityWithSSOAsync<AdministratorDTO, Administrator>(
                dbAdministrator,
                admin => admin.User.KeycloakId.ToString()
            );

            //await UpdateDoctorsAndPatients(administratorWithSSO, 1, int.MaxValue);
            return keycloakService.Serialize(administratorWithSSO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while getting administrator with ID: {id}");
            throw new Exception($"Error occurred while processing administrator with ID: {id}", ex);
        }
    }

    public async Task CreateAsync(AdministratorDTO administratorDto, string keycloakId)
    {
        await userRepository.BeginTransactionAsync();
        try
        {
            var newUserId = Guid.NewGuid();
            var newAdministratorId = Guid.NewGuid();

            var defaultAdminRole = await userRepository.GetRoleByNameAsync("Administrator");
            if (defaultAdminRole == null)
                throw new Exception("Default administrator role not found.");

            var newUser = new User
            {
                Id = newUserId,
                KeycloakId = keycloakId,
                RoleId = defaultAdminRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await userRepository.CreateAsync(newUser);

            var hospital = await hospitalRepository.GetByIdAsync(administratorDto.ManagedHospitalId);
            if (hospital == null)
                throw new Exception("Hospital not found.");

            var administrator = new Administrator
            {
                Id = newAdministratorId,
                UserId = newUserId,
                ManagedHospitalId = administratorDto.ManagedHospitalId,
                IsActive = true
            };
            await administratorRepository.CreateAsync(administrator);

            hospital.Administrators.Add(administrator);
            await hospitalRepository.UpdateAsync(hospital);

            await userRepository.CommitTransactionAsync();
        }
        catch
        {
            await userRepository.RollbackTransactionAsync();
            throw;
        }
    }



    public async Task UpdateAsync(AdministratorDTO administratorDto)
    {
        try
        {
            var administrator = mapper.Map<Administrator>(administratorDto);
            await administratorRepository.UpdateAsync(administrator);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating administrator.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await administratorRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while soft deleting administrator with ID: {id}");
            throw;
        }
    }

    /*private async Task UpdateDoctorsAndPatients(AdministratorDTO administrator, int page, int size)
    {
        if (administrator.Doctors != null && administrator.Doctors.Count != 0)
            await UpdateDoctors(administrator.Doctors, page, size);

        if (administrator.Patients != null && administrator.Patients.Count != 0)
            await UpdatePatients(administrator.Patients, page, size);
    }*/

    private async Task UpdateDoctors(IEnumerable<DoctorDTO> doctors, int page, int size)
    {
        var doctorEntities = await keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
            await doctorRepository.GetAllAsync(page, size),
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
        var patientEntities = await keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
            await patientRepository.GetAllAsync(page, size),
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