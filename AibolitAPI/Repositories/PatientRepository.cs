using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;

namespace AibolitAPI.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(AibolitDbContext context) : base(context)
    {
    }
}