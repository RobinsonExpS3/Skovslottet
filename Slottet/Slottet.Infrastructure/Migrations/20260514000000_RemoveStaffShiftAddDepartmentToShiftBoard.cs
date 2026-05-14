using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStaffShiftAddDepartmentToShiftBoard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StaffShifts");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                table: "ShiftBoards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBoards_DepartmentID",
                table: "ShiftBoards",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftBoards_Departments_DepartmentID",
                table: "ShiftBoards",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftBoards_Departments_DepartmentID",
                table: "ShiftBoards");

            migrationBuilder.DropIndex(
                name: "IX_ShiftBoards_DepartmentID",
                table: "ShiftBoards");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "ShiftBoards");

            migrationBuilder.CreateTable(
                name: "StaffShifts",
                columns: table => new
                {
                    ShiftBoardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffID      = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffShifts", x => new { x.ShiftBoardID, x.StaffID });
                    table.ForeignKey(
                        name: "FK_StaffShifts_ShiftBoards_ShiftBoardID",
                        column: x => x.ShiftBoardID,
                        principalTable: "ShiftBoards",
                        principalColumn: "ShiftBoardID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffShifts_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffShifts_StaffID",
                table: "StaffShifts",
                column: "StaffID");
        }
    }
}
