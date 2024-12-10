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
    public DbSet<ScheduleAdjustment> ScheduleAdjustments { get; set; }
    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---== User and Role ==---
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);


        // ---== Doctor ==---
        modelBuilder.Entity<Doctor>()
            .ToTable("Doctors");

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.WorkSchedule)
            .WithMany()
            .HasForeignKey(d => d.WorkScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Hospital)
            .WithMany(h => h.Doctors)
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


        // ---== Administrator ==---
        modelBuilder.Entity<Administrator>()
            .ToTable("Administrators");

        modelBuilder.Entity<Administrator>()
            .HasOne(a => a.ManagedHospital)
            .WithMany(h => h.Administrators)
            .HasForeignKey(a => a.ManagedHospitalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Administrator>()
            .HasMany(a => a.Doctors)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Administrator>()
            .HasMany(a => a.Patients)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);


        // ---== Patient ==---
        modelBuilder.Entity<Patient>()
            .ToTable("Patients");

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.MedicalRecord)
            .WithOne(mr => mr.Patient)
            .HasForeignKey<Patient>(p => p.MedicalRecordId)
            .OnDelete(DeleteBehavior.Restrict);


        // ---== ScheduleAdjustment ==---
        modelBuilder.Entity<ScheduleAdjustment>()
            .HasOne(sa => sa.WorkSchedule)
            .WithMany(ws => ws.ScheduleAdjustments)
            .HasForeignKey(sa => sa.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---== Hospital ==---
        modelBuilder.Entity<Hospital>()
            .HasMany(h => h.Doctors)
            .WithOne(d => d.Hospital)
            .HasForeignKey(d => d.HospitalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hospital>()
            .HasMany(h => h.Administrators)
            .WithOne(a => a.ManagedHospital)
            .HasForeignKey(a => a.ManagedHospitalId)
            .OnDelete(DeleteBehavior.Restrict);


        // ---== MedicalRecord ==---
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


        // ---== Filters for Active Entities ==---
        // Фильтрация только активных пользователей
        modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

        // Администратор должен быть активным пользователем
        modelBuilder.Entity<Administrator>()
            .HasQueryFilter(a => a.User.IsActive);

        // Доктор и его больница должны быть активными
        modelBuilder.Entity<Doctor>()
            .HasQueryFilter(d => d.User.IsActive && d.Hospital.IsActive);

        // Пациент должен быть активным пользователем
        modelBuilder.Entity<Patient>().HasQueryFilter(p => p.User.IsActive);

        // Фильтр для отображения только активных записей на прием, при условии,
        // что пациент и доктор также активны
        modelBuilder.Entity<Appointment>()
            .HasQueryFilter(a => a.IsActive && a.Patient.IsActive && a.Doctor.IsActive);

        // Медицинская карта должна быть активной
        modelBuilder.Entity<MedicalRecord>().HasQueryFilter(mr => mr.IsActive);


        // Фильтр для активных рецептов, при условии, что пациент активен
        // и врач, выписавший рецепт, является активным пользователем
        modelBuilder.Entity<Prescription>()
            .HasQueryFilter(p => p.IsActive && p.Patient.IsActive && p.PrescribedBy.User.IsActive);

        // Рекомендация должна быть активной, а также пациент и врач, выдавший рекомендацию
        modelBuilder.Entity<Recommendation>()
            .HasQueryFilter(r => r.IsActive && r.Patient.IsActive && r.GivenBy.User.IsActive);

        // Фильтр для отображения только активных больниц
        modelBuilder.Entity<Hospital>()
            .HasQueryFilter(h => h.IsActive);

        // Расписание работы должно быть активным и иметь корректное время
        modelBuilder.Entity<WorkSchedule>()
            .HasQueryFilter(ws => ws.StartTime > TimeSpan.Zero && ws.EndTime > TimeSpan.Zero && ws.IsActive);

        // Фильтр для активных корректировок расписания, связанных с активным расписанием работы
        modelBuilder.Entity<ScheduleAdjustment>()
            .HasQueryFilter(sa => sa.IsActive && sa.WorkSchedule.IsActive);
    }
}