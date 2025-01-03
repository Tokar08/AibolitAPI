using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Interfaces.Services;
using AibolitAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Services;

public class PatientService(
    IPatientRepository patientRepository,
    IMapper mapper,
    ILogger<PatientService> logger,
    IKeycloakService keycloakService,
    IDoctorRepository doctorRepository,
    IAppointmentRepository appointmentRepository,
    IMedicalRecordRepository medicalRecordRepository,
    IRecommendationRepository recommendationRepository,
    IPrescriptionRepository prescriptionRepository,
    INotificationSender notificationSender,
    IEmailTemplateFactory templateFactory,
    ISpecializationService specializationService,
    IFilterService filterService,
    IWorkScheduleRepository workScheduleRepository)
    : IPatientService
{
    public async Task<string> GetAllDoctorsAsyncWithSSO(DoctorFilterDTO? filterDto, int page, int pageSize)
    {
        try
        {
            var dbDoctors = await doctorRepository.GetAllAsync(page, pageSize);

            var doctorEntities = await keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
                dbDoctors,
                doctor => doctor.User.KeycloakId.ToString()
            );

            var filteredDoctors = filterService.ApplyDoctorFilter(doctorEntities, filterDto, dbDoctors);
            return keycloakService.Serialize(filteredDoctors.Where(d => d.IsActive).ToList());
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching doctors with SSO: " + ex.Message);
        }
    }


    public async Task<string> GetAllAppointmentsAsync(
        Guid patientId,
        AppointmentFilterDTO? filterDto,
        int page,
        int size)
    {
        var appointments = await appointmentRepository.GetAppointmentsByPatientIdAsync(patientId);

        var appointmentDTOs = mapper.Map<List<AppointmentDTO>>(appointments);

        var filteredAppointments = filterService
            .ApplyAppointmentFilter(appointmentDTOs, filterDto)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return keycloakService.Serialize(filteredAppointments);
    }


    public async Task CreateAppointmentAsync(Guid doctorId, Guid patientId, DateTime appointmentDate)
    {
        try
        {
            var dbPatient = await GetPatientAsync(patientId);
            var dbDoctor = await GetDoctorAsync(doctorId);

            if (dbPatient.MedicalRecord.DoctorId == null
                && await specializationService.IsFamilyDoctorAsync(dbDoctor.SpecializationId))
                await UpdatePatientDoctorAsync(dbPatient, doctorId);

            var appointment = await CreateAppointmentAsync(
                patientId,
                doctorId,
                dbPatient.MedicalRecordId,
                appointmentDate
            );

            var patientDto = await GetPatientDtoAsync(dbPatient);
            var doctorDto = await GetDoctorDtoAsync(dbDoctor);

            await AddDoctorToPatientIfNeededAsync(dbPatient, dbDoctor);

            await SendAppointmentConfirmationAsync(patientDto, doctorDto, appointment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating appointment.");
            throw new Exception("Error occurred while processing the appointment.", ex);
        }
    }

    public async Task AddDoctorToFavoritesAsync(Guid patientId, Guid doctorId)
    {
        var patient = await patientRepository.GetByIdAsync(patientId);
        if (patient == null)
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

        if (patient.LikedDoctors.All(d => d.Id != doctorId))
        {
            patient.LikedDoctors.Add(doctor);
            await patientRepository.UpdateAsync(patient);
        }
    }

    public async Task RemoveDoctorFromFavoritesAsync(Guid patientId, Guid doctorId)
    {
        var patient = await patientRepository.GetByIdAsync(patientId);

        if (patient == null)
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

        var doctorToRemove = patient.LikedDoctors.FirstOrDefault(d => d.Id == doctorId);

        if (doctorToRemove == null)
            throw new KeyNotFoundException($"Doctor with ID {doctorId} not found in favorites.");

        patient.LikedDoctors.Remove(doctorToRemove);
        await patientRepository.UpdateAsync(patient);
    }

    public async Task<string> GetFavoriteDoctorsWithSSOAsync(Guid patientId, DoctorFilterDTO? filterDto, int page,
        int size)
    {
        var patient = await patientRepository.GetByIdAsync(patientId);

        if (patient == null)
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

        var likedDoctors = patient.LikedDoctors.Where(d => d.IsActive)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        var doctorEntities = await keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
            likedDoctors,
            doctor => doctor.User.KeycloakId.ToString()
        );

        var filteredDoctors = filterService.ApplyDoctorFilter(doctorEntities, filterDto, likedDoctors);
        return keycloakService.Serialize(filteredDoctors.ToList());
    }


    public async Task<string> GetPrescriptionsForPatientAsync(Guid patientId, PrescriptionFilterDTO? filterDto)
    {
        var medicalRecord = await medicalRecordRepository.GetByPatientIdAsync(patientId);
        if (medicalRecord == null)
            throw new KeyNotFoundException("No prescriptions found for this patient.");

        var prescriptions = medicalRecord.Prescriptions.Where(p => p.IsActive).ToList();
        var prescriptionDTOs = mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
        var filteredPrescriptions = filterService.ApplyPrescriptionFilter(prescriptionDTOs, filterDto);

        return keycloakService.Serialize(filteredPrescriptions);
    }


    public async Task<object> GetPrescriptionByIdWithDoctorAsync(Guid patientId, Guid prescriptionId)
    {
        var prescription = await prescriptionRepository.GetByIdAsync(prescriptionId);

        if (prescription == null || !prescription.IsActive)
            throw new KeyNotFoundException($"Prescription with ID: {prescriptionId} not found or inactive.");

        var medicalRecord = await medicalRecordRepository.GetByIdAsync(prescription.MedicalRecordId);

        if (medicalRecord == null) throw new UnauthorizedAccessException("Medical record not found.");

        if (medicalRecord.PatientId != patientId)
            throw new UnauthorizedAccessException("This prescription does not belong to the specified patient.");

        var doctor = await doctorRepository.GetByIdAsync(prescription.DoctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID: {prescription.DoctorId} not found.");

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


    public async Task<string> GetRecommendationsForPatientAsync(Guid patientId, RecommendationFilterDTO? filterDto)
    {
        var medicalRecord = await medicalRecordRepository.GetByPatientIdAsync(patientId);

        if (medicalRecord == null)
            throw new KeyNotFoundException("No recommendations found for this patient.");

        var recommendations = medicalRecord.Recommendations.Where(r => r.IsActive).ToList();
        var recommendationDTOs = mapper.Map<IEnumerable<RecommendationDTO>>(recommendations);

        var filteredRecommendations = filterService.ApplyRecommendationFilter(recommendationDTOs, filterDto);

        return keycloakService.Serialize(filteredRecommendations);
    }


    public async Task<object> GetRecommendationByIdWithDoctorAsync(Guid patientId, Guid recommendationId)
    {
        var recommendation = await recommendationRepository.GetByIdAsync(recommendationId);

        if (recommendation == null || !recommendation.IsActive)
            throw new KeyNotFoundException($"Recommendation with ID: {recommendationId} not found or inactive.");

        var medicalRecord = await medicalRecordRepository.GetByIdAsync(recommendation.MedicalRecordId);

        if (medicalRecord == null || medicalRecord.PatientId != patientId)
            throw new UnauthorizedAccessException("This recommendation does not belong to the specified patient.");

        var doctor = await doctorRepository.GetByIdAsync(recommendation.DoctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with ID: {recommendation.DoctorId} not found.");

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


    public async Task CancelAppointmentAsync(Guid patientId, Guid appointmentId)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

        if (appointment == null)
            throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");

        if (appointment.PatientId != patientId)
            throw new UnauthorizedAccessException("Appointment does not belong to this patient.");

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

    public async Task<string> GetAllPatientsWithSSOAsync(int page, int size, PatientFilterDTO? filterDto = null)
    {
        var dbPatients = await patientRepository.GetAllAsync(page, size);
        var patientEntities = await keycloakService.GetEntitiesWithSSOAsync<PatientDTO, Patient>(
            dbPatients,
            patient => patient.User.KeycloakId.ToString()
        );

        foreach (var patient in patientEntities) await UpdateDoctorsForPatient(patient, page, size);

        patientEntities = filterService.ApplyPatientFilter(patientEntities, filterDto);

        return keycloakService.Serialize(patientEntities);
    }


    public async Task<IEnumerable<PatientDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var patients = await patientRepository.GetAllAsync(page, size,
                patient => patient
                    .Include(p => p.Doctors)
                    .Include(p => p.LikedDoctors)
                    .Include(p => p.User)
            );

            var patientDTOs = mapper.Map<IEnumerable<PatientDTO>>(patients);

            return patientDTOs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all patients.");
            throw;
        }
    }

    public async Task<string> GetByIdAsync(Guid id)
    {
        try
        {
            var dbPatient = await patientRepository.GetByIdAsync(id);
            if (dbPatient == null)
            {
                logger.LogWarning($"Patient with ID: {id} was not found.");
                throw new KeyNotFoundException($"Patient with ID: {id} was not found.");
            }

            var patientWithSSO = await keycloakService.GetEntityWithSSOAsync<PatientDTO, Patient>(
                dbPatient,
                patient => patient.User.KeycloakId.ToString()
            );

            await UpdateDoctorsForPatient(patientWithSSO, 1, int.MaxValue);

            return keycloakService.Serialize(patientWithSSO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while getting patient with ID: {id}");
            throw new Exception($"Error occurred while processing patient with ID: {id}", ex);
        }
    }


    public async Task CreateAsync(PatientDTO patientDto)
    {
        try
        {
            var patient = mapper.Map<Patient>(patientDto);
            await patientRepository.CreateAsync(patient);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating patient.");
            throw;
        }
    }

    public async Task UpdateAsync(PatientDTO patientDto)
    {
        try
        {
            var patient = mapper.Map<Patient>(patientDto);
            await patientRepository.UpdateAsync(patient);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating patient.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await patientRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while soft deleting patient with ID: {id}");
            throw;
        }
    }

    public async Task<IEnumerable<object>> GetAvailableSlotsAsync(Guid doctorId, DateTime date)
    {
        var schedules = await workScheduleRepository.GetSchedulesForDoctorAsync(doctorId, date.DayOfWeek);

        var appointments = await appointmentRepository.GetAllAsync(1, int.MaxValue);
        var appointmentsForDate = appointments
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date && a.IsScheduled)
            .ToList();

        var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

        var slots = new List<object>();

        foreach (var schedule in schedules)
        {
            var currentTimeUtc = date.Date + schedule.StartTime;
            while (currentTimeUtc < date.Date + schedule.EndTime)
            {
                var endTimeUtc = currentTimeUtc.Add(TimeSpan.FromMinutes(20));

                var localStartTime = TimeZoneInfo.ConvertTimeFromUtc(currentTimeUtc, kievTimeZone);
                var localEndTime = TimeZoneInfo.ConvertTimeFromUtc(endTimeUtc, kievTimeZone);

                var isOccupied = appointmentsForDate.Any(a =>
                    a.AppointmentDate >= currentTimeUtc &&
                    a.AppointmentDate < endTimeUtc);

                slots.Add(new
                {
                    StartTime = localStartTime.ToString("HH:mm"),
                    EndTime = localEndTime.ToString("HH:mm"),
                    IsOccupied = isOccupied
                });

                currentTimeUtc = endTimeUtc;
            }
        }

        return slots;
    }


    private async Task<Patient> GetPatientAsync(Guid patientId)
    {
        var dbPatient = await patientRepository.GetByIdAsync(patientId);
        if (dbPatient == null)
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
        return dbPatient;
    }

    private async Task<Doctor> GetDoctorAsync(Guid doctorId)
    {
        var dbDoctor = await doctorRepository.GetByIdAsync(doctorId);
        if (dbDoctor == null)
            throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

        if (!dbDoctor.IsActive)
            throw new UnauthorizedAccessException($"Doctor with ID {doctorId} is inactive.");
        return dbDoctor;
    }

    private async Task UpdatePatientDoctorAsync(Patient dbPatient, Guid doctorId)
    {
        dbPatient.MedicalRecord.DoctorId = doctorId;
        await medicalRecordRepository.UpdateAsync(dbPatient.MedicalRecord);
    }

    private async Task<Appointment> CreateAppointmentAsync(
        Guid patientId,
        Guid doctorId,
        Guid medicalRecordId,
        DateTime appointmentDateUtc)
    {
        if (appointmentDateUtc < DateTime.UtcNow)
            throw new ArgumentException("Appointment date cannot be in the past.");

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            MedicalRecordId = medicalRecordId,
            AppointmentDate = appointmentDateUtc,
            IsScheduled = true,
            IsActive = true
        };

        await appointmentRepository.CreateAsync(appointment);
        return appointment;
    }


    private async Task<PatientDTO> GetPatientDtoAsync(Patient dbPatient)
    {
        return await keycloakService.GetEntityWithSSOAsync<PatientDTO, Patient>(
            dbPatient,
            p => p.User.KeycloakId.ToString()
        );
    }

    private async Task<DoctorDTO> GetDoctorDtoAsync(Doctor dbDoctor)
    {
        return await keycloakService.GetEntityWithSSOAsync<DoctorDTO, Doctor>(
            dbDoctor,
            d => d.User.KeycloakId.ToString()
        );
    }

    private async Task SendAppointmentConfirmationAsync(PatientDTO patientDto, DoctorDTO doctorDto,
        Appointment appointment)
    {
        var ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
        var appointmentTimeUkraine = TimeZoneInfo.ConvertTimeFromUtc(appointment.AppointmentDate, ukraineTimeZone);

        var template = templateFactory.GetTemplate("appointment_confirmation");
        await notificationSender.SendAsync(
            patientDto.Email,
            template,
            new object[]
            {
                patientDto.FirstName,
                doctorDto.PhotoUrl,
                $"{doctorDto.FirstName} {doctorDto.LastName}",
                appointmentTimeUkraine.ToString("dd.MM.yyyy HH:mm"),
                doctorDto.Email,
                doctorDto.PhoneNumber
            }
        );
    }


    private async Task AddDoctorToPatientIfNeededAsync(Patient dbPatient, Doctor dbDoctor)
    {
        if (dbPatient.Doctors.All(d => d.Id != dbDoctor.Id))
        {
            dbPatient.Doctors.Add(dbDoctor);
            await patientRepository.UpdateAsync(dbPatient);
        }
    }

    private async Task SendCancellationNotificationAsync(DoctorDTO doctor, PatientDTO patient, Appointment appointment)
    {
        var template = templateFactory.GetTemplate("appointment_cancellation");
        var ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
        var appointmentTimeUkraine = TimeZoneInfo.ConvertTimeFromUtc(appointment.AppointmentDate, ukraineTimeZone);

        var patientEmailTask = notificationSender.SendAsync(patient.Email, template, new object[]
        {
            appointmentTimeUkraine.ToString("dd.MM.yyyy HH:mm "),
            $"{doctor.FirstName} {doctor.LastName}",
            $"{patient.FirstName} {patient.LastName}"
        });

        var doctorEmailTask = notificationSender.SendAsync(doctor.Email, template, new object[]
        {
            appointmentTimeUkraine.ToString("dd.MM.yyyy HH:mm "),
            $"{doctor.FirstName} {doctor.LastName}",
            $"{patient.FirstName} {patient.LastName}"
        });

        await Task.WhenAll(patientEmailTask, doctorEmailTask);
    }


    private async Task UpdateDoctorsForPatient(PatientDTO patient, int page, int size)
    {
        if (patient.Doctors != null && patient.Doctors.Count > 0)
        {
            var dbDoctors = await doctorRepository.GetAllAsync(page, size);
            var doctorEntities = await keycloakService.GetEntitiesWithSSOAsync<DoctorDTO, Doctor>(
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
}