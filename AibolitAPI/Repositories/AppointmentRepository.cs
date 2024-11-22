using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;

namespace AibolitAPI.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AibolitDbContext context) : base(context)
    {
    }
}