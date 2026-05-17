using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ScopeAssignmentsToShiftBoard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaff_Departments_DepartmentID",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaff_SpecialResponsibilities_SpecialResponsibilityID",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaff_Staffs_StaffID",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecialResponsibilityStaff",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropColumn(
                name: "WorkDayDate",
                table: "StaffResidentStatuses");

            migrationBuilder.RenameTable(
                name: "SpecialResponsibilityStaff",
                newName: "SpecialResponsibilityStaffs");

            migrationBuilder.RenameIndex(
                name: "IX_SpecialResponsibilityStaff_StaffID",
                table: "SpecialResponsibilityStaffs",
                newName: "IX_SpecialResponsibilityStaffs_StaffID");

            migrationBuilder.RenameIndex(
                name: "IX_SpecialResponsibilityStaff_DepartmentID",
                table: "SpecialResponsibilityStaffs",
                newName: "IX_SpecialResponsibilityStaffs_DepartmentID");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftBoardID",
                table: "StaffResidentStatuses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "StaffResidentStatuses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftBoardID",
                table: "StaffPhones",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                table: "SpecialResponsibilities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftBoardID",
                table: "SpecialResponsibilityStaffs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Sæt DepartmentID på eksisterende SpecialResponsibilities til Skoven
            migrationBuilder.Sql(
                "UPDATE SpecialResponsibilities SET DepartmentID = '628610AE-47F1-43B3-A750-553C86226439'");

            // Ryd eksisterende assignments — ShiftBoardID = Guid.Empty er ugyldigt som FK
            migrationBuilder.Sql("DELETE FROM StaffResidentStatuses");
            migrationBuilder.Sql("DELETE FROM StaffPhones");
            migrationBuilder.Sql("DELETE FROM SpecialResponsibilityStaffs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses",
                columns: new[] { "StaffID", "ResidentStatusID", "ShiftBoardID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones",
                columns: new[] { "StaffID", "PhoneID", "ShiftBoardID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecialResponsibilityStaffs",
                table: "SpecialResponsibilityStaffs",
                columns: new[] { "SpecialResponsibilityID", "StaffID", "ShiftBoardID" });

            migrationBuilder.CreateIndex(
                name: "IX_StaffResidentStatuses_ShiftBoardID",
                table: "StaffResidentStatuses",
                column: "ShiftBoardID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffPhones_ShiftBoardID",
                table: "StaffPhones",
                column: "ShiftBoardID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialResponsibilities_DepartmentID",
                table: "SpecialResponsibilities",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialResponsibilityStaffs_ShiftBoardID",
                table: "SpecialResponsibilityStaffs",
                column: "ShiftBoardID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilities_Departments_DepartmentID",
                table: "SpecialResponsibilities",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaffs_Departments_DepartmentID",
                table: "SpecialResponsibilityStaffs",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaffs_ShiftBoards_ShiftBoardID",
                table: "SpecialResponsibilityStaffs",
                column: "ShiftBoardID",
                principalTable: "ShiftBoards",
                principalColumn: "ShiftBoardID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaffs_SpecialResponsibilities_SpecialResponsibilityID",
                table: "SpecialResponsibilityStaffs",
                column: "SpecialResponsibilityID",
                principalTable: "SpecialResponsibilities",
                principalColumn: "SpecialResponsibilityID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaffs_Staffs_StaffID",
                table: "SpecialResponsibilityStaffs",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffPhones_ShiftBoards_ShiftBoardID",
                table: "StaffPhones",
                column: "ShiftBoardID",
                principalTable: "ShiftBoards",
                principalColumn: "ShiftBoardID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffResidentStatuses_ShiftBoards_ShiftBoardID",
                table: "StaffResidentStatuses",
                column: "ShiftBoardID",
                principalTable: "ShiftBoards",
                principalColumn: "ShiftBoardID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilities_Departments_DepartmentID",
                table: "SpecialResponsibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaffs_Departments_DepartmentID",
                table: "SpecialResponsibilityStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaffs_ShiftBoards_ShiftBoardID",
                table: "SpecialResponsibilityStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaffs_SpecialResponsibilities_SpecialResponsibilityID",
                table: "SpecialResponsibilityStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaffs_Staffs_StaffID",
                table: "SpecialResponsibilityStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffPhones_ShiftBoards_ShiftBoardID",
                table: "StaffPhones");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffResidentStatuses_ShiftBoards_ShiftBoardID",
                table: "StaffResidentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_StaffResidentStatuses_ShiftBoardID",
                table: "StaffResidentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones");

            migrationBuilder.DropIndex(
                name: "IX_StaffPhones_ShiftBoardID",
                table: "StaffPhones");

            migrationBuilder.DropIndex(
                name: "IX_SpecialResponsibilities_DepartmentID",
                table: "SpecialResponsibilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecialResponsibilityStaffs",
                table: "SpecialResponsibilityStaffs");

            migrationBuilder.DropIndex(
                name: "IX_SpecialResponsibilityStaffs_ShiftBoardID",
                table: "SpecialResponsibilityStaffs");

            migrationBuilder.DropColumn(
                name: "ShiftBoardID",
                table: "StaffResidentStatuses");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "StaffResidentStatuses");

            migrationBuilder.DropColumn(
                name: "ShiftBoardID",
                table: "StaffPhones");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "SpecialResponsibilities");

            migrationBuilder.DropColumn(
                name: "ShiftBoardID",
                table: "SpecialResponsibilityStaffs");

            migrationBuilder.RenameTable(
                name: "SpecialResponsibilityStaffs",
                newName: "SpecialResponsibilityStaff");

            migrationBuilder.RenameIndex(
                name: "IX_SpecialResponsibilityStaffs_StaffID",
                table: "SpecialResponsibilityStaff",
                newName: "IX_SpecialResponsibilityStaff_StaffID");

            migrationBuilder.RenameIndex(
                name: "IX_SpecialResponsibilityStaffs_DepartmentID",
                table: "SpecialResponsibilityStaff",
                newName: "IX_SpecialResponsibilityStaff_DepartmentID");

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkDayDate",
                table: "StaffResidentStatuses",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses",
                columns: new[] { "StaffID", "ResidentStatusID", "WorkDayDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones",
                columns: new[] { "StaffID", "PhoneID", "AssignedAt" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecialResponsibilityStaff",
                table: "SpecialResponsibilityStaff",
                columns: new[] { "SpecialResponsibilityID", "StaffID", "AssignedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaff_Departments_DepartmentID",
                table: "SpecialResponsibilityStaff",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaff_SpecialResponsibilities_SpecialResponsibilityID",
                table: "SpecialResponsibilityStaff",
                column: "SpecialResponsibilityID",
                principalTable: "SpecialResponsibilities",
                principalColumn: "SpecialResponsibilityID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaff_Staffs_StaffID",
                table: "SpecialResponsibilityStaff",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
