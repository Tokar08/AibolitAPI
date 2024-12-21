using AibolitAPI.Interfaces;

namespace AibolitAPI.Services;

public class AppointmentSuccessCheckService : BackgroundService
{
    private readonly ILogger<AppointmentSuccessCheckService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AppointmentSuccessCheckService(
        ILogger<AppointmentSuccessCheckService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Executing appointment success check service...");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
                var doctorRepository = scope.ServiceProvider.GetRequiredService<IDoctorRepository>();

                var appointments = await appointmentRepository.GetAllAsync(1, int.MaxValue);

                var nowUtc = DateTime.UtcNow;

                foreach (var appointment in appointments)
                {
                    var appointmentTimeUtc = appointment.AppointmentDate;

                    Console.WriteLine($"Appointment ID: {appointment.Id}");
                    Console.WriteLine($"Appointment Time (UTC): {appointmentTimeUtc}");

                    //TODO: для теста 2 минуты, должно быть 20 минут
                    var successTime = appointmentTimeUtc.AddMinutes(2);

                    Console.WriteLine($"Success Time (UTC): {successTime}");

                    if (nowUtc >= successTime && appointment.IsScheduled && appointment.IsActive)
                    {
                        Console.WriteLine($"Appointment {appointment.Id} is now eligible to be marked as successful.");

                        appointment.IsScheduled = false;
                        await appointmentRepository.UpdateAsync(appointment);

                        var doctor = await doctorRepository.GetByIdAsync(appointment.DoctorId);
                        if (doctor != null)
                        {
                            doctor.VisitCount++;
                            await doctorRepository.UpdateAsync(doctor);
                        }

                        _logger.LogInformation($"Appointment {appointment.Id} marked as successful.");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Skipping appointment {appointment.Id} (not yet eligible for success check or already processed).");
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}