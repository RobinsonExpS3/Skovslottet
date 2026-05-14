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
            // ── 1. Fjern FK fra Medicines → Residents (skal droppes før kolonneændringer) ──
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Residents_ResidentID",
                table: "Medicines");

            // ── 2. Fjern index på ResidentID (kan blokere kolonneændringer) ──────
            migrationBuilder.DropIndex(
                name: "IX_Medicines_ResidentID",
                table: "Medicines");

            // ── 3. Ryd dato-specifik seed-data (format skifter helt) ──────────────
            migrationBuilder.Sql("DELETE FROM Medicines");

            // ── 4. Drop de tre gamle datetime-kolonner ────────────────────────────
            migrationBuilder.DropColumn(name: "MedicineGivenTime",      table: "Medicines");
            migrationBuilder.DropColumn(name: "MedicineRegisteredTime", table: "Medicines");
            migrationBuilder.DropColumn(name: "MedicineTime",           table: "Medicines");

            // ── 5. Tilføj ScheduledTime (time) ───────────────────────────────────
            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledTime",
                table: "Medicines",
                type: "time",
                nullable: false,
                defaultValueSql: "'00:00:00'");

            // ── 6. Genopret index og FK ───────────────────────────────────────────
            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ResidentID",
                table: "Medicines",
                column: "ResidentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Residents_ResidentID",
                table: "Medicines",
                column: "ResidentID",
                principalTable: "Residents",
                principalColumn: "ResidentID",
                onDelete: ReferentialAction.Restrict);

            // ── 7. Opret MedicineLogs tabel ───────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "MedicineLogs",
                columns: table => new
                {
                    MedicineLogID  = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date           = table.Column<DateOnly>(type: "date", nullable: false),
                    GivenTime      = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisteredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MedicineID     = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            // ── 8. Tilføj PNMedication til PNs ─────────────────────────────────
            migrationBuilder.AddColumn<string>(
                name: "PNMedication",
                table: "PNs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MedicineLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Residents_ResidentID",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_ResidentID",
                table: "Medicines");

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

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ResidentID",
                table: "Medicines",
                column: "ResidentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Residents_ResidentID",
                table: "Medicines",
                column: "ResidentID",
                principalTable: "Residents",
                principalColumn: "ResidentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(name: "PNMedication", table: "PNs");
        }
    }
}
