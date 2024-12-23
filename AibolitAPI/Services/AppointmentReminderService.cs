using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Newtonsoft.Json;

namespace AibolitAPI.Services;

public class AppointmentReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private bool _hasSentDiseaseSearchInfo;
    private bool _hasSentFriendlyReminder;

    public AppointmentReminderService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("AppointmentReminderService: Executing appointment reminder service...");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
                var hospitalService = scope.ServiceProvider.GetRequiredService<IHospitalService>();
                var patientService = scope.ServiceProvider.GetRequiredService<IPatientService>();
                var doctorService = scope.ServiceProvider.GetRequiredService<IDoctorService>();
                var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();
                var templateFactory = scope.ServiceProvider.GetRequiredService<IEmailTemplateFactory>();

                var upcomingAppointments = await appointmentRepository.GetUpcomingAppointmentsAsync();

                var ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
                var nowUkraine = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ukraineTimeZone);

                foreach (var appointment in upcomingAppointments)
                {
                    var appointmentTimeUkraine =
                        TimeZoneInfo.ConvertTimeFromUtc(appointment.AppointmentDate, ukraineTimeZone);
                    var reminderTimeUkraine = appointmentTimeUkraine.AddMinutes(-1);

                    if (!IsReminderTime(nowUkraine, reminderTimeUkraine))
                        continue;

                    Console.WriteLine("AppointmentReminderService: Sending reminder...");

                    var patientJson = await patientService.GetByIdAsync(appointment.PatientId);
                    var patient = JsonConvert.DeserializeObject<PatientDTO>(patientJson);

                    var doctorJson = await doctorService.GetByIdAsync(appointment.DoctorId);
                    var doctor = JsonConvert.DeserializeObject<DoctorDTO>(doctorJson);

                    var hospitalJson = await hospitalService.GetByIdAsync(doctor.HospitalId);
                    var hospital = JsonConvert.DeserializeObject<HospitalDTO>(hospitalJson);
                    var hospitalAddress = hospital?.Address ?? "Адрес не указан";

                    var template = templateFactory.GetTemplate("appointment_reminder");
                    await notificationSender.SendAsync(
                        patient.Email,
                        template,
                        new object[]
                        {
                            patient.FirstName,
                            doctor.PhotoUrl,
                            $"{doctor.FirstName} {doctor.LastName}",
                            appointmentTimeUkraine.ToString("dd.MM.yyyy HH:mm"),
                            hospitalAddress
                        }
                    );
                }

                await ProcessFriendlyReminder(notificationSender, patientService, templateFactory, nowUkraine);
                await ProcessDiseaseSearchInfo(notificationSender, patientService, templateFactory, nowUkraine);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }


    private async Task ProcessFriendlyReminder(
        INotificationSender notificationSender,
        IPatientService patientService,
        IEmailTemplateFactory templateFactory,
        DateTime nowUkraine)
    {
        if (IsFriendlyReminderTime(nowUkraine) && !_hasSentFriendlyReminder)
        {
            Console.WriteLine("AppointmentReminderService: Sending Friendly Reminder...");

            var allPatientsJson = await patientService.GetAllPatientsWithSSOAsync(1, int.MaxValue);
            var patients = JsonConvert.DeserializeObject<List<PatientDTO>>(allPatientsJson);

            foreach (var patient in patients)
            {
                var template = templateFactory.GetTemplate("friendly_reminder");
                await notificationSender.SendAsync(
                    patient.Email,
                    template,
                    new object[] { patient.FirstName }
                );
            }

            _hasSentFriendlyReminder = true;
        }
        else if (!IsFriendlyReminderTime(nowUkraine))
        {
            _hasSentFriendlyReminder = false;
        }
    }

    private async Task ProcessDiseaseSearchInfo(
        INotificationSender notificationSender,
        IPatientService patientService,
        IEmailTemplateFactory templateFactory,
        DateTime nowUkraine)
    {
        if (IsDeseaseSearchInfoTime(nowUkraine) && !_hasSentDiseaseSearchInfo)
        {
            Console.WriteLine("AppointmentReminderService: Sending Desease Search Info...");

            var allPatientsJson = await patientService.GetAllPatientsWithSSOAsync(1, int.MaxValue);
            var patients = JsonConvert.DeserializeObject<List<PatientDTO>>(allPatientsJson);

            foreach (var patient in patients)
            {
                var template = templateFactory.GetTemplate("disease_search_info");
                await notificationSender.SendAsync(
                    patient.Email,
                    template,
                    new object[] { patient.FirstName }
                );
            }

            _hasSentDiseaseSearchInfo = true;
        }
        else if (!IsDeseaseSearchInfoTime(nowUkraine))
        {
            _hasSentDiseaseSearchInfo = false;
        }
    }

    private bool IsReminderTime(DateTime nowUkraine, DateTime reminderTime)
    {
        return reminderTime >= nowUkraine && reminderTime <= nowUkraine.AddSeconds(30);
    }

    private bool IsFriendlyReminderTime(DateTime nowUkraine)
    {
        return nowUkraine.DayOfWeek == DayOfWeek.Monday && nowUkraine.TimeOfDay.Hours == 12 &&
               nowUkraine.TimeOfDay.Minutes == 0;
    }

    private bool IsDeseaseSearchInfoTime(DateTime nowUkraine)
    {
        return nowUkraine.DayOfWeek == DayOfWeek.Friday && nowUkraine.TimeOfDay.Hours == 9 &&
               nowUkraine.TimeOfDay.Minutes == 0;
    }
}