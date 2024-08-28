namespace AibolitAPI.Models;

public class Prescription
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    public Guid DoctorId { get; set; }
    public virtual Doctor PrescribedBy { get; set; }
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public string MedicationName { get; set; }
    public string Dosage { get; set; }
    public string Instructions { get; set; }
}