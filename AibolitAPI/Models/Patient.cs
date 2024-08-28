namespace AibolitAPI.Models;

public class Patient : User
{
    public virtual ICollection<Doctor> Doctors { get; set; }
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    public virtual ICollection<Doctor> LikedDoctors { get; set; } 
}