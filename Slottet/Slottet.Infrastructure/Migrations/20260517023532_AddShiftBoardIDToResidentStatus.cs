using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftBoardIDToResidentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ShiftBoardID",
                table: "ResidentStatuses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResidentStatuses_ShiftBoardID",
                table: "ResidentStatuses",
                column: "ShiftBoardID");

            migrationBuilder.AddForeignKey(
                name: "FK_ResidentStatuses_ShiftBoards_ShiftBoardID",
                table: "ResidentStatuses",
                column: "ShiftBoardID",
                principalTable: "ShiftBoards",
                principalColumn: "ShiftBoardID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResidentStatuses_ShiftBoards_ShiftBoardID",
                table: "ResidentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ResidentStatuses_ShiftBoardID",
                table: "ResidentStatuses");

            migrationBuilder.DropColumn(
                name: "ShiftBoardID",
                table: "ResidentStatuses");
        }
    }
}
