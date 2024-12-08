namespace AibolitAPI.Models;

public class Doctor : HospitalStaff
{
    public string Specialization { get; set; }
    public Guid WorkScheduleId { get; set; }
    public virtual WorkSchedule WorkSchedule { get; set; }

    public Guid HospitalId { get; set; }
    public virtual Hospital Hospital { get; set; }

    public int YearsOfExperience { get; set; }
    public string Education { get; set; }
    public string PhotoUrl { get; set; }

    public int VisitCount { get; set; }
    public virtual ICollection<Patient> Patients { get; set; }
    public virtual ICollection<Patient> LikedByPatients { get; set; }
    public bool IsActive { get; set; }
}