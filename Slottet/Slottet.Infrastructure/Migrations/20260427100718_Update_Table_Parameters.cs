using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Table_Parameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PNStatus",
                table: "PNs",
                newName: "PNReason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PNReason",
                table: "PNs",
                newName: "PNStatus");
        }
    }
}
