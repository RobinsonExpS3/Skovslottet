using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MedicineSoftDeleteAndProtectMedicineLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Soft delete flag på Medicines
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Medicines",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Ændr FK MedicineLogs → Medicines fra Cascade til Restrict.
            // Beskytter MedicineLog historik mod kaskade-sletning.
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineLogs_Medicines_MedicineID",
                table: "MedicineLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineLogs_Medicines_MedicineID",
                table: "MedicineLogs",
                column: "MedicineID",
                principalTable: "Medicines",
                principalColumn: "MedicineID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineLogs_Medicines_MedicineID",
                table: "MedicineLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineLogs_Medicines_MedicineID",
                table: "MedicineLogs",
                column: "MedicineID",
                principalTable: "Medicines",
                principalColumn: "MedicineID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Medicines");
        }
    }
}
