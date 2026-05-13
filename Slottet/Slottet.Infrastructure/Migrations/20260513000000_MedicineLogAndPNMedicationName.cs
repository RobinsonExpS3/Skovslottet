using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Slottet.Infrastructure.Data;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    [Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(SlottetDBContext))]
    [Migration("20260513000000_MedicineLogAndPNMedicationName")]
    public partial class MedicineLogAndPNMedicationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── 1. Drop den gamle Medicines tabel helt (dato-baseret design) ──────
            //    FK fra Medicines → Residents droppes automatisk med tabellen.
            migrationBuilder.DropTable(name: "Medicines");

            // ── 2. Opret ny Medicines tabel med ScheduledTime (TimeOnly) ──────────
            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    MedicineID    = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ResidentID    = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.MedicineID);
                    table.ForeignKey(
                        name: "FK_Medicines_Residents_ResidentID",
                        column: x => x.ResidentID,
                        principalTable: "Residents",
                        principalColumn: "ResidentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ResidentID",
                table: "Medicines",
                column: "ResidentID");

            // ── 3. Opret MedicineLogs tabel ───────────────────────────────────────
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

            // ── 4. Tilføj MedicationName til PNs ─────────────────────────────────
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
            migrationBuilder.DropTable(name: "MedicineLogs");
            migrationBuilder.DropTable(name: "Medicines");

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    MedicineID             = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicineTime           = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicineGivenTime      = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicineRegisteredTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResidentID             = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.MedicineID);
                    table.ForeignKey(
                        name: "FK_Medicines_Residents_ResidentID",
                        column: x => x.ResidentID,
                        principalTable: "Residents",
                        principalColumn: "ResidentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.DropColumn(name: "MedicationName", table: "PNs");
        }
    }
}
