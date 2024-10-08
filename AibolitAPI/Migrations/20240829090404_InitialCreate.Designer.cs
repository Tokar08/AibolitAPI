﻿// <auto-generated />
using System;
using AibolitAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AibolitAPI.Migrations
{
    [DbContext(typeof(AibolitDbContext))]
    [Migration("20240829090404_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AibolitAPI.Models.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsScheduled")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MedicalRecordId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.HasIndex("MedicalRecordId");

                    b.HasIndex("PatientId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("AibolitAPI.Models.Hospital", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("AdministratorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChiefDoctorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AdministratorId")
                        .IsUnique();

                    b.HasIndex("ChiefDoctorId");

                    b.ToTable("Hospitals");
                });

            modelBuilder.Entity("AibolitAPI.Models.MedicalRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("MedicalRecords");
                });

            modelBuilder.Entity("AibolitAPI.Models.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("NotificationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("AibolitAPI.Models.Prescription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Dosage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Instructions")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MedicalRecordId")
                        .HasColumnType("uuid");

                    b.Property<string>("MedicationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("PrescriptionDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.HasIndex("MedicalRecordId");

                    b.HasIndex("PatientId");

                    b.ToTable("Prescriptions");
                });

            modelBuilder.Entity("AibolitAPI.Models.Recommendation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MedicalRecordId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("RecommendationDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.HasIndex("MedicalRecordId");

                    b.HasIndex("PatientId");

                    b.ToTable("Recommendations");
                });

            modelBuilder.Entity("AibolitAPI.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("AibolitAPI.Models.WorkSchedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DaysOfWeek")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.ToTable("WorkSchedules");
                });

            modelBuilder.Entity("DoctorPatient", b =>
                {
                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.HasKey("DoctorId", "PatientId");

                    b.HasIndex("PatientId");

                    b.ToTable("DoctorPatient");
                });

            modelBuilder.Entity("PatientDoctorLikes", b =>
                {
                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.HasKey("DoctorId", "PatientId");

                    b.HasIndex("PatientId");

                    b.ToTable("PatientDoctorLikes");
                });

            modelBuilder.Entity("AibolitAPI.Models.Administrator", b =>
                {
                    b.HasBaseType("AibolitAPI.Models.User");

                    b.Property<Guid>("ManagedHospitalId")
                        .HasColumnType("uuid");

                    b.ToTable("Administrators", (string)null);
                });

            modelBuilder.Entity("AibolitAPI.Models.Doctor", b =>
                {
                    b.HasBaseType("AibolitAPI.Models.User");

                    b.Property<Guid?>("AdministratorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("HospitalId")
                        .HasColumnType("uuid");

                    b.Property<string>("Specialization")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("VisitCount")
                        .HasColumnType("integer");

                    b.Property<Guid>("WorkScheduleId")
                        .HasColumnType("uuid");

                    b.HasIndex("AdministratorId");

                    b.HasIndex("HospitalId");

                    b.HasIndex("WorkScheduleId");

                    b.ToTable("Doctors", (string)null);
                });

            modelBuilder.Entity("AibolitAPI.Models.Patient", b =>
                {
                    b.HasBaseType("AibolitAPI.Models.User");

                    b.Property<Guid?>("AdministratorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MedicalRecordId")
                        .HasColumnType("uuid");

                    b.HasIndex("AdministratorId");

                    b.HasIndex("MedicalRecordId")
                        .IsUnique();

                    b.ToTable("Patients", (string)null);
                });

            modelBuilder.Entity("AibolitAPI.Models.Appointment", b =>
                {
                    b.HasOne("AibolitAPI.Models.Doctor", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.MedicalRecord", "MedicalRecord")
                        .WithMany("Appointments")
                        .HasForeignKey("MedicalRecordId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");

                    b.Navigation("MedicalRecord");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("AibolitAPI.Models.Hospital", b =>
                {
                    b.HasOne("AibolitAPI.Models.Administrator", "Administrator")
                        .WithOne("ManagedHospital")
                        .HasForeignKey("AibolitAPI.Models.Hospital", "AdministratorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AibolitAPI.Models.Doctor", "ChiefDoctor")
                        .WithMany()
                        .HasForeignKey("ChiefDoctorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Administrator");

                    b.Navigation("ChiefDoctor");
                });

            modelBuilder.Entity("AibolitAPI.Models.MedicalRecord", b =>
                {
                    b.HasOne("AibolitAPI.Models.Doctor", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("AibolitAPI.Models.Notification", b =>
                {
                    b.HasOne("AibolitAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AibolitAPI.Models.Prescription", b =>
                {
                    b.HasOne("AibolitAPI.Models.Doctor", "PrescribedBy")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.MedicalRecord", "MedicalRecord")
                        .WithMany("Prescriptions")
                        .HasForeignKey("MedicalRecordId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MedicalRecord");

                    b.Navigation("Patient");

                    b.Navigation("PrescribedBy");
                });

            modelBuilder.Entity("AibolitAPI.Models.Recommendation", b =>
                {
                    b.HasOne("AibolitAPI.Models.Doctor", "GivenBy")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.MedicalRecord", "MedicalRecord")
                        .WithMany("Recommendations")
                        .HasForeignKey("MedicalRecordId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GivenBy");

                    b.Navigation("MedicalRecord");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("DoctorPatient", b =>
                {
                    b.HasOne("AibolitAPI.Models.Doctor", null)
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.Patient", null)
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PatientDoctorLikes", b =>
                {
                    b.HasOne("AibolitAPI.Models.Doctor", null)
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.Patient", null)
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AibolitAPI.Models.Doctor", b =>
                {
                    b.HasOne("AibolitAPI.Models.Administrator", null)
                        .WithMany("Doctors")
                        .HasForeignKey("AdministratorId");

                    b.HasOne("AibolitAPI.Models.Hospital", "Hospital")
                        .WithMany("Staff")
                        .HasForeignKey("HospitalId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AibolitAPI.Models.WorkSchedule", "WorkSchedule")
                        .WithMany()
                        .HasForeignKey("WorkScheduleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Hospital");

                    b.Navigation("WorkSchedule");
                });

            modelBuilder.Entity("AibolitAPI.Models.Patient", b =>
                {
                    b.HasOne("AibolitAPI.Models.Administrator", null)
                        .WithMany("Patients")
                        .HasForeignKey("AdministratorId");

                    b.HasOne("AibolitAPI.Models.MedicalRecord", "MedicalRecord")
                        .WithOne("Patient")
                        .HasForeignKey("AibolitAPI.Models.Patient", "MedicalRecordId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MedicalRecord");
                });

            modelBuilder.Entity("AibolitAPI.Models.Hospital", b =>
                {
                    b.Navigation("Staff");
                });

            modelBuilder.Entity("AibolitAPI.Models.MedicalRecord", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Patient")
                        .IsRequired();

                    b.Navigation("Prescriptions");

                    b.Navigation("Recommendations");
                });

            modelBuilder.Entity("AibolitAPI.Models.Administrator", b =>
                {
                    b.Navigation("Doctors");

                    b.Navigation("ManagedHospital")
                        .IsRequired();

                    b.Navigation("Patients");
                });
#pragma warning restore 612, 618
        }
    }
}
