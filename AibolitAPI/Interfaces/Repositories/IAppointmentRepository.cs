using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<List<Appointment>> GetUpcomingAppointmentsAsync();
    Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);
    Task CancelAppointmentAsync(Guid appointmentId);
}