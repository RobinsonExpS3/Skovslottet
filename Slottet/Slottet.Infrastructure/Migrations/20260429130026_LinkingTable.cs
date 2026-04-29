using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilities_ShiftBoards_ShiftBoardID",
                table: "SpecialResponsibilities");

            migrationBuilder.DropIndex(
                name: "IX_SpecialResponsibilities_ShiftBoardID",
                table: "SpecialResponsibilities");

            migrationBuilder.DropColumn(
                name: "ShiftBoardID",
                table: "SpecialResponsibilities");

            migrationBuilder.CreateTable(
                name: "SpecialResponsibilityStaff",
                columns: table => new
                {
                    SpecialResponsibilityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialResponsibilityStaff", x => new { x.SpecialResponsibilityID, x.StaffID, x.AssignedAt });
                    table.ForeignKey(
                        name: "FK_SpecialResponsibilityStaff_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecialResponsibilityStaff_SpecialResponsibilities_SpecialResponsibilityID",
                        column: x => x.SpecialResponsibilityID,
                        principalTable: "SpecialResponsibilities",
                        principalColumn: "SpecialResponsibilityID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecialResponsibilityStaff_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialResponsibilityStaff_DepartmentID",
                table: "SpecialResponsibilityStaff",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialResponsibilityStaff_StaffID",
                table: "SpecialResponsibilityStaff",
                column: "StaffID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialResponsibilityStaff");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftBoardID",
                table: "SpecialResponsibilities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SpecialResponsibilities_ShiftBoardID",
                table: "SpecialResponsibilities",
                column: "ShiftBoardID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilities_ShiftBoards_ShiftBoardID",
                table: "SpecialResponsibilities",
                column: "ShiftBoardID",
                principalTable: "ShiftBoards",
                principalColumn: "ShiftBoardID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
