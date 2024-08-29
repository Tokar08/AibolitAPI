namespace AibolitAPI.DTOs;

public class PrescriptionDTO
{
    public Guid Id { get; set; }
    public string PatientFullName { get; set; }
    public string DoctorFullName { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public string MedicationName { get; set; }
    public string Dosage { get; set; }
    public string Instructions { get; set; }
    public bool IsActive { get; set; }
}