using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;

namespace AibolitAPI.Repositories;

public class HospitalRepository : Repository<Hospital>, IHospitalRepository
{
    public HospitalRepository(AibolitDbContext context) : base(context)
    {
    }
}