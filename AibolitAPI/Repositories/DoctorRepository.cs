using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;

namespace AibolitAPI.Repositories;

public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    public DoctorRepository(AibolitDbContext context) : base(context)
    {
    }
}