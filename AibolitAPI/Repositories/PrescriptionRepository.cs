using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;

namespace AibolitAPI.Repositories;

public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(AibolitDbContext context) : base(context)
    {
    }
}