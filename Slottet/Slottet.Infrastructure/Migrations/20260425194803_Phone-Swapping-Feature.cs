using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PhoneSwappingFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffPhones",
                columns: table => new
                {
                    StaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPhones", x => new { x.StaffID, x.PhoneID, x.AssignedAt });
                    table.ForeignKey(
                        name: "FK_StaffPhones_Phones_PhoneID",
                        column: x => x.PhoneID,
                        principalTable: "Phones",
                        principalColumn: "PhoneID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffPhones_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffPhones_PhoneID",
                table: "StaffPhones",
                column: "PhoneID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffPhones");
        }
    }
}
