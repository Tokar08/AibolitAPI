namespace AibolitAPI.DTOs;

public class PrescriptionDTO
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public PatientDTO Patient { get; set; } 
    public Guid DoctorId { get; set; }
    public DoctorDTO PrescribedBy { get; set; } 
    public Guid MedicalRecordId { get; set; }
    public MedicalRecordDTO MedicalRecord { get; set; } 
    public DateTime PrescriptionDate { get; set; }
    public string MedicationName { get; set; }
    public string Dosage { get; set; }
    public string Instructions { get; set; }
    public bool IsActive { get; set; }
}