using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Data;

public class AibolitDbContext : DbContext
{
    public AibolitDbContext(DbContextOptions<AibolitDbContext> options)
        : base(options)
    {
    }

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
        modelBuilder.Entity<User>().UseTpcMappingStrategy();

        modelBuilder.Entity<Doctor>()
            .ToTable("Doctors");

        modelBuilder.Entity<Administrator>()
            .ToTable("Administrators");

        modelBuilder.Entity<Patient>()
            .ToTable("Patients");

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.WorkSchedule)
            .WithMany()
            .HasForeignKey(d => d.WorkScheduleId)
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

        modelBuilder.Entity<Hospital>()
            .HasMany(h => h.Staff)
            .WithOne(d => d.Hospital)
            .HasForeignKey(d => d.HospitalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hospital>()
            .HasOne(h => h.ChiefDoctor)
            .WithMany()
            .HasForeignKey(h => h.ChiefDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hospital>()
            .HasOne(h => h.Administrator)
            .WithOne(a => a.ManagedHospital)
            .HasForeignKey<Hospital>(h => h.AdministratorId)
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

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.MedicalRecord)
            .WithOne(mr => mr.Patient)
            .HasForeignKey<Patient>(p => p.MedicalRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
