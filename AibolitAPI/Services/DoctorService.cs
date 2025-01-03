using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Interfaces.Services;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class DoctorService(
    IDoctorRepository doctorRepository,
    IPatientRepository patientRepository,
    IMapper mapper,
    ILogger<DoctorService> logger,
    IKeycloakService keycloakService,
    IMedicalRecordRepository medicalRecordRepository,
    IRecommendationRepository recommendationRepository,
    IPrescriptionRepository prescriptionRepository,
    IAppointmentRepository appointmentRepository,
    INotificationSender notificationSender,
    IEmailTemplateFactory templateFactory,
    IUserRepository userRepository,
    ISpecializationRepository specializationRepository,
    IWorkScheduleRepository workScheduleRepository,
    IFilterService filterService,
    ICloudStorageService cloudStorageService)
    : IDoctorService
{
    public async Task<IEnumerable<DoctorDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var doctors = await doctorRepository.GetAllAsync(page, size);
            return mapper.Map<IEnumerable<DoctorDTO>>(doctors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all doctors.");
            throw;
        }
    }

    public async Task<string> GetAllDoctorsWithSSOAsync(int page, int size, DoctorFilterDTO? filterDto = null)
    {
        var dbDoctors = await doctorRepository.GetAllAsync(page, size);
        var doctorEntities = await keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
            dbDoctors,
            doctor => doctor.User.KeycloakId.ToString()
        );

        foreach (var doctor in doctorEntities)
            await UpdatePatientsForDoctor(doctor, page, size);


        doctorEntities = filterService.ApplyDoctorFilter(doctorEntities, filterDto, dbDoctors);
        return keycloakService.Serialize(doctorEntities.Where(d => d.IsActive));
    }


    public async Task<string> GetByIdAsync(Guid id)
    {
        try
        {
            var dbDoctor = await doctorRepository.GetByIdAsync(id);
            if (dbDoctor == null)
            {
                logger.LogWarning($"Doctor with ID: {id} was not found.");
                throw new KeyNotFoundException($"Doctor with ID: {id} was not found.");
            }

            var doctorWithSSO = await keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
                dbDoctor,
                doctor => doctor.User.KeycloakId.ToString()
            );

            await UpdatePatientsForDoctor(doctorWithSSO, 1, int.MaxValue);

            return keycloakService.Serialize(doctorWithSSO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while getting doctor with ID: {id}");
            throw new Exception($"Error occurred while processing doctor with ID: {id}", ex);
        }
    }

    public async Task<string> GetPatientsForDoctorAsync(Guid doctorId, int page, int size, PatientFilterDTO? filterDto)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} not found.");

        var patients = doctor.Patients
            .Where(p => p.Doctors.Any(d => d.Id == doctorId))
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        var patientEntities = await keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
            patients,
            patient => patient.User.KeycloakId.ToString()
        );

        var filteredPatients = filterService.ApplyPatientFilter(patientEntities, filterDto);

        return keycloakService.Serialize(filteredPatients.ToList());
    }


    public async Task<string> GetPatientByIdAsync(Guid doctorId, Guid patientId)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} not found.");

        var patient = await patientRepository.GetByIdAsync(patientId);
        if (patient == null || patient.Doctors.All(d => d.Id != doctorId))
            throw new KeyNotFoundException(
                $"Patient with ID: {patientId} is not assigned to Doctor with ID: {doctorId}.");

        var patientEntity =
            await keycloakService.GetEntityWithSSOAsync<PatientDTO, Patient>(patient,
                p => p.User.KeycloakId.ToString());
        return keycloakService.Serialize(patientEntity);
    }

    public async Task<string> GetPrescriptionsForPatientAsync(Guid doctorId, Guid patientId,
        PrescriptionFilterDTO? filterDto)
    {
        var medicalRecord = await medicalRecordRepository.GetByPatientIdAsync(patientId);

        if (medicalRecord == null)
            throw new KeyNotFoundException("Prescriptions not found for this patient and doctor.");

        var prescriptions = medicalRecord.Prescriptions
            .Where(p => p.IsActive)
            .ToList();

        var prescriptionDTOs = mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
        var filteredPrescriptions = filterService.ApplyPrescriptionFilter(prescriptionDTOs, filterDto);

        return keycloakService.Serialize(filteredPrescriptions.ToList());
    }


    public async Task<string> GetRecommendationsForPatientAsync(Guid doctorId, Guid patientId,
        RecommendationFilterDTO? filterDto)
    {
        var medicalRecord = await medicalRecordRepository.GetByPatientIdAsync(patientId);

        if (medicalRecord == null)
            throw new KeyNotFoundException($"No recommendations found for patient with ID {patientId}.");

        var recommendations = medicalRecord.Recommendations
            .Where(r => r.IsActive)
            .ToList();

        var recommendationDTOs = mapper.Map<IEnumerable<RecommendationDTO>>(recommendations);
        var filteredRecommendations = filterService.ApplyRecommendationFilter(recommendationDTOs, filterDto);

        return keycloakService.Serialize(filteredRecommendations);
    }


    public async Task<object> GetPatientPrescriptionByIdAsync(Guid doctorId, Guid patientId, Guid prescriptionId)
    {
        var prescription = await prescriptionRepository.GetByIdAsync(prescriptionId);

        if (prescription == null || !prescription.IsActive)
            throw new KeyNotFoundException($"Prescription with ID: {prescriptionId} not found or inactive.");

        var medicalRecord = await medicalRecordRepository.GetByIdAsync(prescription.MedicalRecordId);

        if (medicalRecord == null || medicalRecord.PatientId != patientId)
            throw new UnauthorizedAccessException(
                "This prescription does not belong to the specified patient or doctor.");

        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} not found.");

        var doctorDto = await keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
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

    public async Task<object> GetPatientRecommendationByIdAsync(Guid doctorId, Guid patientId, Guid recommendationId)
    {
        var recommendation = await recommendationRepository.GetByIdAsync(recommendationId);

        if (recommendation == null || !recommendation.IsActive)
            throw new KeyNotFoundException($"Recommendation with ID: {recommendationId} not found or inactive.");

        var medicalRecord = await medicalRecordRepository.GetByIdAsync(recommendation.MedicalRecordId);

        if (medicalRecord == null || medicalRecord.PatientId != patientId)
            throw new UnauthorizedAccessException(
                "This recommendation does not belong to the specified patient or doctor.");

        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID: {doctorId} not found.");

        var doctorDto = await keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
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

    public async Task<IEnumerable<AppointmentDTO>> GetScheduledAppointmentsByDoctorIdAsync(Guid doctorId,
        AppointmentFilterDTO? filterDto, int page, int size)
    {
        var appointments = await appointmentRepository.GetAppointmentsByDoctorIdAsync(doctorId);

        var appointmentDTOs = mapper.Map<IEnumerable<AppointmentDTO>>(appointments);

        var filteredAppointments = filterService
            .ApplyAppointmentFilter(appointmentDTOs, filterDto)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return filteredAppointments;
    }


    public async Task CancelAppointmentAsync(Guid doctorId, Guid appointmentId)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

        if (appointment == null)
            throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");

        if (appointment.DoctorId != doctorId)
            throw new UnauthorizedAccessException("Appointment does not belong to this doctor.");

        if (!appointment.IsScheduled)
            throw new InvalidOperationException("Appointment is not scheduled or already cancelled.");

        await appointmentRepository.CancelAppointmentAsync(appointmentId);

        var doctor = await keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
            appointment.Doctor,
            d => d.User.KeycloakId
        );

        var patient = await keycloakService.GetEntityWithSSOAsync<PatientDTO, Patient>(
            appointment.Patient,
            p => p.User.KeycloakId
        );


        await SendCancellationNotificationAsync(doctor, patient, appointment);
    }


    public async Task CreateAsync(DoctorDTO doctorDto, string keycloakId, Stream? photoStream, string? fileName)
    {
        await userRepository.BeginTransactionAsync();
        try
        {
            var newUserId = Guid.NewGuid();
            var newDoctorId = Guid.NewGuid();

            var defaultDoctorRole = await userRepository.GetRoleByNameAsync("Doctor");
            if (defaultDoctorRole == null)
                throw new Exception("Default doctor role not found.");

            var newUser = new User
            {
                Id = newUserId,
                KeycloakId = keycloakId,
                RoleId = defaultDoctorRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await userRepository.CreateAsync(newUser);

            var specialization =
                await specializationRepository.GetByTitleAsync(doctorDto.SpecializationTitle);
            if (specialization == null)
                throw new Exception($"Specialization '{doctorDto.SpecializationTitle}' not found.");

            var doctor = new Doctor
            {
                Id = newDoctorId,
                UserId = newUserId,
                SpecializationId = specialization.Id,
                HospitalId = doctorDto.HospitalId,
                YearsOfExperience = doctorDto.YearsOfExperience,
                Education = doctorDto.Education,
                VisitCount = doctorDto.VisitCount,
                IsActive = true,
                WorkSchedules = new List<WorkSchedule>(),
                Patients = new List<Patient>(),
                LikedByPatients = new List<Patient>()
            };

            if (photoStream != null && !string.IsNullOrWhiteSpace(fileName))
            {
                var uploadedPhotoUrl = await cloudStorageService.UploadPhotoAsync(photoStream, fileName);
                doctor.PhotoUrl = uploadedPhotoUrl;
            }

            await doctorRepository.CreateAsync(doctor);

            await userRepository.CommitTransactionAsync();
        }
        catch
        {
            await userRepository.RollbackTransactionAsync();
            throw;
        }
    }


    public async Task<string> UpdateWithPhotoAsync(Guid id, DoctorDTO doctorDto, Stream? photoStream, string? fileName)
    {
        try
        {
            var existingDoctor = await doctorRepository.GetByIdAsync(id)
                                 ?? throw new KeyNotFoundException($"Doctor with ID {id} not found.");

            var oldPhotoUrl = existingDoctor.PhotoUrl;

            if (!string.IsNullOrWhiteSpace(doctorDto.SpecializationTitle))
            {
                var specialization = await specializationRepository
                    .GetByTitleAsync(doctorDto.SpecializationTitle);

                if (specialization == null)
                    throw new KeyNotFoundException(
                        $"Specialization with title '{doctorDto.SpecializationTitle}' not found.");

                existingDoctor.SpecializationId = specialization.Id;
            }

            mapper.Map(doctorDto, existingDoctor);

            if (photoStream != null && !string.IsNullOrWhiteSpace(fileName))
            {
                if (!string.IsNullOrWhiteSpace(oldPhotoUrl))
                    await cloudStorageService.DeletePhotoAsync(oldPhotoUrl);

                existingDoctor.PhotoUrl = await cloudStorageService.UploadPhotoAsync(photoStream, fileName);
            }

            if (string.IsNullOrWhiteSpace(existingDoctor.PhotoUrl))
                throw new InvalidOperationException("Doctor photo cannot be empty.");

            await doctorRepository.UpdateAsync(existingDoctor);

            return existingDoctor.PhotoUrl;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while updating doctor with ID: {id}");
            throw;
        }
    }


    public async Task UpdateDoctorSchedulesAsync(Guid doctorId, IEnumerable<WorkScheduleDTO> scheduleDtos)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

        var newScheduleIds = scheduleDtos.Select(s => s.Id).ToList();

        var existingWorkSchedules = await workScheduleRepository.GetByIdsAsync(newScheduleIds);

        var currentSchedules = doctor.WorkSchedules.ToList();

        foreach (var oldSchedule in currentSchedules.Where(oldSchedule => !newScheduleIds.Contains(oldSchedule.Id)))
            doctor.WorkSchedules.Remove(oldSchedule);

        foreach (var schedule in existingWorkSchedules.Where(schedule =>
                     doctor.WorkSchedules.All(ws => ws.Id != schedule.Id)))
            doctor.WorkSchedules.Add(schedule);

        await doctorRepository.UpdateAsync(doctor);
    }

    public async Task<List<WorkScheduleDTO>> GetWorkSchedulesForDoctorAsync(Guid doctorId)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);

        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

        var schedules = doctor.WorkSchedules;
        var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

        foreach (var schedule in schedules)
        {
            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);

            var startTimeUtc = DateTime.SpecifyKind(today + schedule.StartTime, DateTimeKind.Utc);
            var endTimeUtc = DateTime.SpecifyKind(today + schedule.EndTime, DateTimeKind.Utc);

            schedule.StartTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeUtc, kievTimeZone).TimeOfDay;
            schedule.EndTime = TimeZoneInfo.ConvertTimeFromUtc(endTimeUtc, kievTimeZone).TimeOfDay;
        }

        var scheduleDtos = mapper.Map<List<WorkScheduleDTO>>(schedules);

        return scheduleDtos;
    }


    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            var doctor = await doctorRepository.GetByIdAsync(id);
            if (doctor != null && !string.IsNullOrWhiteSpace(doctor.PhotoUrl))
                await cloudStorageService.DeletePhotoAsync(doctor.PhotoUrl);

            await doctorRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while soft deleting doctor with ID: {id}");
            throw;
        }
    }

    private async Task SendCancellationNotificationAsync(DoctorDTO doctor, PatientDTO patient, Appointment appointment)
    {
        var template = templateFactory.GetTemplate("appointment_cancellation");

        var patientEmailTask = notificationSender.SendAsync(patient.Email, template, new object[]
        {
            appointment.AppointmentDate.ToString("f"),
            $"{doctor.FirstName} {doctor.LastName}",
            $"{patient.FirstName} {patient.LastName}"
        });

        var doctorEmailTask = notificationSender.SendAsync(doctor.Email, template, new object[]
        {
            appointment.AppointmentDate.ToString("f"),
            $"{doctor.FirstName} {doctor.LastName}",
            $"{patient.FirstName} {patient.LastName}"
        });

        await Task.WhenAll(patientEmailTask, doctorEmailTask);
    }


    private async Task UpdatePatientsForDoctor(DoctorDTO doctor, int page, int size)
    {
        if (doctor.Patients != null && doctor.Patients.Count > 0)
        {
            var dbPatients = await patientRepository.GetAllAsync(page, size);
            var patientEntities = await keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
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