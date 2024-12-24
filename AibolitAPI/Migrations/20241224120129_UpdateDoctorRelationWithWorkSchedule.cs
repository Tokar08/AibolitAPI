using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AibolitAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorRelationWithWorkSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specializations_SpecializationId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_WorkSchedules_WorkScheduleId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_WorkScheduleId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkScheduleId",
                table: "Doctors");

            migrationBuilder.CreateTable(
                name: "DoctorWorkSchedule",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkScheduleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorWorkSchedule", x => new { x.DoctorId, x.WorkScheduleId });
                    table.ForeignKey(
                        name: "FK_DoctorWorkSchedule_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorWorkSchedule_WorkSchedules_WorkScheduleId",
                        column: x => x.WorkScheduleId,
                        principalTable: "WorkSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorWorkSchedule_WorkScheduleId",
                table: "DoctorWorkSchedule",
                column: "WorkScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specializations_SpecializationId",
                table: "Doctors",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specializations_SpecializationId",
                table: "Doctors");

            migrationBuilder.DropTable(
                name: "DoctorWorkSchedule");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkScheduleId",
                table: "Doctors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_WorkScheduleId",
                table: "Doctors",
                column: "WorkScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specializations_SpecializationId",
                table: "Doctors",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_WorkSchedules_WorkScheduleId",
                table: "Doctors",
                column: "WorkScheduleId",
                principalTable: "WorkSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
