using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_PNStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffPNs",
                columns: table => new
                {
                    StaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PNID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPNs", x => new { x.StaffID, x.PNID });
                    table.ForeignKey(
                        name: "FK_StaffPNs_PNs_PNID",
                        column: x => x.PNID,
                        principalTable: "PNs",
                        principalColumn: "PNID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffPNs_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffPNs_PNID",
                table: "StaffPNs",
                column: "PNID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffPNs");
        }
    }
}
