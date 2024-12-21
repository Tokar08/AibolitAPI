using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IMedicalRecordRepository : IRepository<MedicalRecord>
{
    Task<MedicalRecord> GetByPatientIdAsync(Guid patientId);
}