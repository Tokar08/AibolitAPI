using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Data;

public class AibolitDbContext : DbContext
{
    public AibolitDbContext(DbContextOptions<AibolitDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<WorkSchedule> WorkSchedules { get; set; }
    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Doctor>()
            .ToTable("Doctors");

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.WorkSchedule)
            .WithMany()
            .HasForeignKey(d => d.WorkScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Hospital)
            .WithMany(h => h.Staff)
            .HasForeignKey(d => d.HospitalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasMany(d => d.Patients)
            .WithMany(p => p.Doctors)
            .UsingEntity<Dictionary<string, object>>(
                "DoctorPatient",
                j => j.HasOne<Patient>().WithMany().HasForeignKey("PatientId"),
                j => j.HasOne<Doctor>().WithMany().HasForeignKey("DoctorId"));

        modelBuilder.Entity<Doctor>()
            .HasMany(d => d.LikedByPatients)
            .WithMany(p => p.LikedDoctors)
            .UsingEntity<Dictionary<string, object>>(
                "PatientDoctorLikes",
                j => j.HasOne<Patient>().WithMany().HasForeignKey("PatientId"),
                j => j.HasOne<Doctor>().WithMany().HasForeignKey("DoctorId"));
        
        modelBuilder.Entity<Administrator>()
            .ToTable("Administrators");

        modelBuilder.Entity<Administrator>()
            .HasOne(a => a.ManagedHospital)
            .WithOne(h => h.Administrator)
            .HasForeignKey<Hospital>(h => h.AdministratorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Administrator>()
            .HasMany(a => a.Doctors)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Administrator>()
            .HasMany(a => a.Patients)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Patient>()
            .ToTable("Patients");

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.MedicalRecord)
            .WithOne(mr => mr.Patient)
            .HasForeignKey<Patient>(p => p.MedicalRecordId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Hospital>()
            .HasOne(h => h.ChiefDoctor)
            .WithMany()
            .HasForeignKey(h => h.ChiefDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hospital>()
            .HasMany(h => h.Staff)
            .WithOne(d => d.Hospital)
            .HasForeignKey(d => d.HospitalId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<MedicalRecord>()
            .HasMany(mr => mr.Appointments)
            .WithOne(a => a.MedicalRecord)
            .HasForeignKey(a => a.MedicalRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicalRecord>()
            .HasMany(mr => mr.Prescriptions)
            .WithOne(p => p.MedicalRecord)
            .HasForeignKey(p => p.MedicalRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicalRecord>()
            .HasMany(mr => mr.Recommendations)
            .WithOne(r => r.MedicalRecord)
            .HasForeignKey(r => r.MedicalRecordId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Фильтрация только активных пользователей
        modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

        // Администратор должен быть активным
        modelBuilder.Entity<Administrator>()
            .HasQueryFilter(a => a.User.IsActive);

        // Доктор и его больница должны быть активными
        modelBuilder.Entity<Doctor>()
            .HasQueryFilter(d => d.User.IsActive && d.Hospital.IsActive);

        // Пациент и его медицинская карта должны быть активными
        modelBuilder.Entity<Patient>()
            .HasQueryFilter(p => p.User.IsActive && p.MedicalRecord.IsActive);

        // Запись на прием должна быть активной и пациент с доктором активными
        modelBuilder.Entity<Appointment>()
            .HasQueryFilter(a => a.IsActive && a.Patient.IsActive && a.Doctor.IsActive);

        // Медицинская карта должна быть активной
        modelBuilder.Entity<MedicalRecord>()
            .HasQueryFilter(mr => mr.IsActive && mr.Patient.IsActive);

        // Рецепт должен быть активным, пациент и доктор тоже
        modelBuilder.Entity<Prescription>()
            .HasQueryFilter(p => p.IsActive && p.Patient.IsActive && p.PrescribedBy.User.IsActive);

        // Рекомендация должна быть активной, пациент и врач тоже
        modelBuilder.Entity<Recommendation>()
            .HasQueryFilter(r => r.IsActive && r.Patient.IsActive && r.GivenBy.User.IsActive);

        // Больница должна быть активной
        modelBuilder.Entity<Hospital>()
            .HasQueryFilter(h => h.IsActive);

        // Уведомление должно быть активным, пользователь активен
        modelBuilder.Entity<Notification>()
            .HasQueryFilter(n => n.IsActive && n.User.IsActive);

        // Время работы не может быть нулевым
        modelBuilder.Entity<WorkSchedule>()
            .HasQueryFilter(ws => ws.StartTime > TimeSpan.Zero && ws.EndTime > TimeSpan.Zero);
    }
}
