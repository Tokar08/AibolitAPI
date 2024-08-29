using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AibolitAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ManagedHospitalId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    NotificationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsScheduled = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorPatient",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorPatient", x => new { x.DoctorId, x.PatientId });
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Specialization = table.Column<string>(type: "text", nullable: false),
                    WorkScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    HospitalId = table.Column<Guid>(type: "uuid", nullable: false),
                    VisitCount = table.Column<int>(type: "integer", nullable: false),
                    AdministratorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctors_Administrators_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Administrators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Doctors_WorkSchedules_WorkScheduleId",
                        column: x => x.WorkScheduleId,
                        principalTable: "WorkSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hospitals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    ChiefDoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdministratorId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospitals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hospitals_Administrators_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Administrators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hospitals_Doctors_ChiefDoctorId",
                        column: x => x.ChiefDoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdministratorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Administrators_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Administrators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Patients_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientDoctorLikes",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDoctorLikes", x => new { x.DoctorId, x.PatientId });
                    table.ForeignKey(
                        name: "FK_PatientDoctorLikes_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientDoctorLikes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescriptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MedicationName = table.Column<string>(type: "text", nullable: false),
                    Dosage = table.Column<string>(type: "text", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prescriptions_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    RecommendationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recommendations_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recommendations_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recommendations_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_MedicalRecordId",
                table: "Appointments",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorPatient_PatientId",
                table: "DoctorPatient",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AdministratorId",
                table: "Doctors",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_HospitalId",
                table: "Doctors",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_WorkScheduleId",
                table: "Doctors",
                column: "WorkScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_AdministratorId",
                table: "Hospitals",
                column: "AdministratorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_ChiefDoctorId",
                table: "Hospitals",
                column: "ChiefDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDoctorLikes_PatientId",
                table: "PatientDoctorLikes",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AdministratorId",
                table: "Patients",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicalRecordId",
                table: "Patients",
                column: "MedicalRecordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_DoctorId",
                table: "Prescriptions",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_MedicalRecordId",
                table: "Prescriptions",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PatientId",
                table: "Prescriptions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_DoctorId",
                table: "Recommendations",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_MedicalRecordId",
                table: "Recommendations",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_PatientId",
                table: "Recommendations",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_MedicalRecords_MedicalRecordId",
                table: "Appointments",
                column: "MedicalRecordId",
                principalTable: "MedicalRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPatient_Doctors_DoctorId",
                table: "DoctorPatient",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPatient_Patients_PatientId",
                table: "DoctorPatient",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Hospitals_HospitalId",
                table: "Doctors",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Doctors_ChiefDoctorId",
                table: "Hospitals");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DoctorPatient");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PatientDoctorLikes");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Hospitals");

            migrationBuilder.DropTable(
                name: "WorkSchedules");

            migrationBuilder.DropTable(
                name: "Administrators");
        }
    }
}
