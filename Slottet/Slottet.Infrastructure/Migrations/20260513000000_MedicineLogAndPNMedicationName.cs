using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MedicineLogAndPNMedicationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── 1. Ryd eksisterende Medicine-data (dato-specifik format udskiftes) ──
            migrationBuilder.Sql("DELETE FROM Medicines");

            // ── 2. Fjern gamle datetime-kolonner fra Medicines ────────────────────
            migrationBuilder.DropColumn(
                name: "MedicineGivenTime",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "MedicineRegisteredTime",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "MedicineTime",
                table: "Medicines");

            // ── 3. Tilføj ScheduledTime (time) til Medicines ──────────────────────
            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledTime",
                table: "Medicines",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            // ── 4. Opret MedicineLogs tabel ───────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "MedicineLogs",
                columns: table => new
                {
                    MedicineLogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    GivenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisteredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MedicineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineLogs", x => x.MedicineLogID);
                    table.ForeignKey(
                        name: "FK_MedicineLogs_Medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "Medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineLogs_MedicineID",
                table: "MedicineLogs",
                column: "MedicineID");

            // ── 5. Tilføj MedicationName til PNs ─────────────────────────────────
            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "PNs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ── Fjern MedicineLogs ────────────────────────────────────────────────
            migrationBuilder.DropTable(name: "MedicineLogs");

            // ── Gendan Medicine kolonner ──────────────────────────────────────────
            migrationBuilder.DropColumn(name: "ScheduledTime", table: "Medicines");

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicineTime",
                table: "Medicines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicineGivenTime",
                table: "Medicines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicineRegisteredTime",
                table: "Medicines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // ── Fjern MedicationName fra PNs ──────────────────────────────────────
            migrationBuilder.DropColumn(name: "MedicationName", table: "PNs");
        }
    }
}
