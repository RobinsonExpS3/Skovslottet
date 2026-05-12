using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteStaffHistoryLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaff_Staffs_StaffID",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffPhones_Staffs_StaffID",
                table: "StaffPhones");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffPNs_Staffs_StaffID",
                table: "StaffPNs");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffResidentStatuses_Staffs_StaffID",
                table: "StaffResidentStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffShifts_Staffs_StaffID",
                table: "StaffShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffShifts",
                table: "StaffShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffPNs",
                table: "StaffPNs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecialResponsibilityStaff",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffShifts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "StaffShiftID",
                table: "StaffShifts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StaffShifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffResidentStatuses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "StaffResidentStatusID",
                table: "StaffResidentStatuses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StaffResidentStatuses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffPNs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "StaffPNID",
                table: "StaffPNs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StaffPNs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffPhones",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "StaffPhoneID",
                table: "StaffPhones",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StaffPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "SpecialResponsibilityStaff",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialResponsibilityStaffID",
                table: "SpecialResponsibilityStaff",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SpecialResponsibilityStaff",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE [StaffShifts] SET [StaffShiftID] = NEWID() WHERE [StaffShiftID] IS NULL");
            migrationBuilder.Sql("UPDATE [StaffResidentStatuses] SET [StaffResidentStatusID] = NEWID() WHERE [StaffResidentStatusID] IS NULL");
            migrationBuilder.Sql("UPDATE [StaffPNs] SET [StaffPNID] = NEWID() WHERE [StaffPNID] IS NULL");
            migrationBuilder.Sql("UPDATE [StaffPhones] SET [StaffPhoneID] = NEWID() WHERE [StaffPhoneID] IS NULL");
            migrationBuilder.Sql("UPDATE [SpecialResponsibilityStaff] SET [SpecialResponsibilityStaffID] = NEWID() WHERE [SpecialResponsibilityStaffID] IS NULL");

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffShiftID",
                table: "StaffShifts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffResidentStatusID",
                table: "StaffResidentStatuses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffPNID",
                table: "StaffPNs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffPhoneID",
                table: "StaffPhones",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SpecialResponsibilityStaffID",
                table: "SpecialResponsibilityStaff",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffShifts",
                table: "StaffShifts",
                column: "StaffShiftID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses",
                column: "StaffResidentStatusID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffPNs",
                table: "StaffPNs",
                column: "StaffPNID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones",
                column: "StaffPhoneID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecialResponsibilityStaff",
                table: "SpecialResponsibilityStaff",
                column: "SpecialResponsibilityStaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffShifts_ShiftBoardID",
                table: "StaffShifts",
                column: "ShiftBoardID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffResidentStatuses_StaffID",
                table: "StaffResidentStatuses",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffPNs_StaffID",
                table: "StaffPNs",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffPhones_StaffID",
                table: "StaffPhones",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialResponsibilityStaff_SpecialResponsibilityID",
                table: "SpecialResponsibilityStaff",
                column: "SpecialResponsibilityID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaff_Staffs_StaffID",
                table: "SpecialResponsibilityStaff",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffPhones_Staffs_StaffID",
                table: "StaffPhones",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffPNs_Staffs_StaffID",
                table: "StaffPNs",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffResidentStatuses_Staffs_StaffID",
                table: "StaffResidentStatuses",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffShifts_Staffs_StaffID",
                table: "StaffShifts",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialResponsibilityStaff_Staffs_StaffID",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffPhones_Staffs_StaffID",
                table: "StaffPhones");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffPNs_Staffs_StaffID",
                table: "StaffPNs");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffResidentStatuses_Staffs_StaffID",
                table: "StaffResidentStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffShifts_Staffs_StaffID",
                table: "StaffShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffShifts",
                table: "StaffShifts");

            migrationBuilder.DropIndex(
                name: "IX_StaffShifts_ShiftBoardID",
                table: "StaffShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_StaffResidentStatuses_StaffID",
                table: "StaffResidentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffPNs",
                table: "StaffPNs");

            migrationBuilder.DropIndex(
                name: "IX_StaffPNs_StaffID",
                table: "StaffPNs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones");

            migrationBuilder.DropIndex(
                name: "IX_StaffPhones_StaffID",
                table: "StaffPhones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecialResponsibilityStaff",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropIndex(
                name: "IX_SpecialResponsibilityStaff_SpecialResponsibilityID",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropColumn(
                name: "StaffShiftID",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "StaffResidentStatusID",
                table: "StaffResidentStatuses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StaffResidentStatuses");

            migrationBuilder.DropColumn(
                name: "StaffPNID",
                table: "StaffPNs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StaffPNs");

            migrationBuilder.DropColumn(
                name: "StaffPhoneID",
                table: "StaffPhones");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StaffPhones");

            migrationBuilder.DropColumn(
                name: "SpecialResponsibilityStaffID",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SpecialResponsibilityStaff");

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffShifts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffResidentStatuses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffPNs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "StaffPhones",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffID",
                table: "SpecialResponsibilityStaff",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffShifts",
                table: "StaffShifts",
                columns: new[] { "ShiftBoardID", "StaffID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffResidentStatuses",
                table: "StaffResidentStatuses",
                columns: new[] { "StaffID", "ResidentStatusID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffPNs",
                table: "StaffPNs",
                columns: new[] { "StaffID", "PNID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffPhones",
                table: "StaffPhones",
                columns: new[] { "StaffID", "PhoneID", "AssignedAt" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecialResponsibilityStaff",
                table: "SpecialResponsibilityStaff",
                columns: new[] { "SpecialResponsibilityID", "StaffID", "AssignedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialResponsibilityStaff_Staffs_StaffID",
                table: "SpecialResponsibilityStaff",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffPhones_Staffs_StaffID",
                table: "StaffPhones",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffPNs_Staffs_StaffID",
                table: "StaffPNs",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffResidentStatuses_Staffs_StaffID",
                table: "StaffResidentStatuses",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffShifts_Staffs_StaffID",
                table: "StaffShifts",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
