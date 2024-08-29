using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<Appointment> CreateAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task<IEnumerable<Appointment>> GetAllByDoctorIdAsync(Guid doctorId, int page, int size);
    Task<IEnumerable<Appointment>> GetAllByUserIdAsync(Guid userId, int page, int size);
    Task<Appointment> GetByIdForDoctorAsync(Guid doctorId, Guid appointmentId);
    Task<Appointment> GetByIdForPatientAsync(Guid patientId, Guid appointmentId);
    Task DeleteAsync(Guid appointmentId);
}