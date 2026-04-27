using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PNs_ResidentStatuses_ResidentStatusID",
                table: "PNs");

            migrationBuilder.RenameColumn(
                name: "PNTime",
                table: "PNs",
                newName: "PNRegisteredTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResidentStatusID",
                table: "PNs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "PNGivenTime",
                table: "PNs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ResidentID",
                table: "PNs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PNs_ResidentID",
                table: "PNs",
                column: "ResidentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PNs_ResidentStatuses_ResidentStatusID",
                table: "PNs",
                column: "ResidentStatusID",
                principalTable: "ResidentStatuses",
                principalColumn: "ResidentStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_PNs_Residents_ResidentID",
                table: "PNs",
                column: "ResidentID",
                principalTable: "Residents",
                principalColumn: "ResidentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PNs_ResidentStatuses_ResidentStatusID",
                table: "PNs");

            migrationBuilder.DropForeignKey(
                name: "FK_PNs_Residents_ResidentID",
                table: "PNs");

            migrationBuilder.DropIndex(
                name: "IX_PNs_ResidentID",
                table: "PNs");

            migrationBuilder.DropColumn(
                name: "PNGivenTime",
                table: "PNs");

            migrationBuilder.DropColumn(
                name: "ResidentID",
                table: "PNs");

            migrationBuilder.RenameColumn(
                name: "PNRegisteredTime",
                table: "PNs",
                newName: "PNTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResidentStatusID",
                table: "PNs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PNs_ResidentStatuses_ResidentStatusID",
                table: "PNs",
                column: "ResidentStatusID",
                principalTable: "ResidentStatuses",
                principalColumn: "ResidentStatusID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
