using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Hest_Migration : Migration
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
                newName: "PNGivenTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResidentStatusID",
                table: "PNs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ResidentID",
                table: "PNs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    KeyValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedByStaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PerformedByStaffName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PerformedAtTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogID);
                });

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

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_PNs_ResidentID",
                table: "PNs");

            migrationBuilder.DropColumn(
                name: "ResidentID",
                table: "PNs");

            migrationBuilder.RenameColumn(
                name: "PNGivenTime",
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
