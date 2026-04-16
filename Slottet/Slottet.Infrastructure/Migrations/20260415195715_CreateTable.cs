using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slottet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentID);
                });

            migrationBuilder.CreateTable(
                name: "GroceryDays",
                columns: table => new
                {
                    GroceryDayID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroceryDayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroceryDays", x => x.GroceryDayID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethodID);
                });

            migrationBuilder.CreateTable(
                name: "RiskLevels",
                columns: table => new
                {
                    RiskLevelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiskLevelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskLevels", x => x.RiskLevelID);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBoards",
                columns: table => new
                {
                    ShiftBoardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBoards", x => x.ShiftBoardID);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentTasks",
                columns: table => new
                {
                    DepartmentTaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentTaskName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentTasks", x => x.DepartmentTaskID);
                    table.ForeignKey(
                        name: "FK_DepartmentTasks_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    PhoneID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.PhoneID);
                    table.ForeignKey(
                        name: "FK_Phones_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Initials = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staffs_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    ResidentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResidentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    GroceryDayID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.ResidentID);
                    table.ForeignKey(
                        name: "FK_Residents_GroceryDays_GroceryDayID",
                        column: x => x.GroceryDayID,
                        principalTable: "GroceryDays",
                        principalColumn: "GroceryDayID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecialResponsibilities",
                columns: table => new
                {
                    SpecialResponsibilityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShiftBoardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialResponsibilities", x => x.SpecialResponsibilityID);
                    table.ForeignKey(
                        name: "FK_SpecialResponsibilities_ShiftBoards_ShiftBoardID",
                        column: x => x.ShiftBoardID,
                        principalTable: "ShiftBoards",
                        principalColumn: "ShiftBoardID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffShifts",
                columns: table => new
                {
                    ShiftBoardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    MedicineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicineTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicineGivenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicineRegisteredTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResidentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.MedicineID);
                    table.ForeignKey(
                        name: "FK_Medicines_Residents_ResidentID",
                        column: x => x.ResidentID,
                        principalTable: "Residents",
                        principalColumn: "ResidentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResidentPaymentMethods",
                columns: table => new
                {
                    ResidentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidentPaymentMethods", x => new { x.ResidentID, x.PaymentMethodID });
                    table.ForeignKey(
                        name: "FK_ResidentPaymentMethods_PaymentMethods_PaymentMethodID",
                        column: x => x.PaymentMethodID,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidentPaymentMethods_Residents_ResidentID",
                        column: x => x.ResidentID,
                        principalTable: "Residents",
                        principalColumn: "ResidentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResidentStatuses",
                columns: table => new
                {
                    ResidentStatusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResidentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiskLevelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidentStatuses", x => x.ResidentStatusID);
                    table.ForeignKey(
                        name: "FK_ResidentStatuses_Residents_ResidentID",
                        column: x => x.ResidentID,
                        principalTable: "Residents",
                        principalColumn: "ResidentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidentStatuses_RiskLevels_RiskLevelID",
                        column: x => x.RiskLevelID,
                        principalTable: "RiskLevels",
                        principalColumn: "RiskLevelID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PNs",
                columns: table => new
                {
                    PNID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PNTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PNStatus = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ResidentStatusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PNs", x => x.PNID);
                    table.ForeignKey(
                        name: "FK_PNs_ResidentStatuses_ResidentStatusID",
                        column: x => x.ResidentStatusID,
                        principalTable: "ResidentStatuses",
                        principalColumn: "ResidentStatusID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffResidentStatuses",
                columns: table => new
                {
                    StaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResidentStatusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffResidentStatuses", x => new { x.StaffID, x.ResidentStatusID });
                    table.ForeignKey(
                        name: "FK_StaffResidentStatuses_ResidentStatuses_ResidentStatusID",
                        column: x => x.ResidentStatusID,
                        principalTable: "ResidentStatuses",
                        principalColumn: "ResidentStatusID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffResidentStatuses_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTasks_DepartmentID",
                table: "DepartmentTasks",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ResidentID",
                table: "Medicines",
                column: "ResidentID");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_DepartmentID",
                table: "Phones",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PNs_ResidentStatusID",
                table: "PNs",
                column: "ResidentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentPaymentMethods_PaymentMethodID",
                table: "ResidentPaymentMethods",
                column: "PaymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_Residents_GroceryDayID",
                table: "Residents",
                column: "GroceryDayID");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentStatuses_ResidentID",
                table: "ResidentStatuses",
                column: "ResidentID");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentStatuses_RiskLevelID",
                table: "ResidentStatuses",
                column: "RiskLevelID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialResponsibilities_ShiftBoardID",
                table: "SpecialResponsibilities",
                column: "ShiftBoardID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffResidentStatuses_ResidentStatusID",
                table: "StaffResidentStatuses",
                column: "ResidentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_DepartmentID",
                table: "Staffs",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffShifts_StaffID",
                table: "StaffShifts",
                column: "StaffID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentTasks");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "Phones");

            migrationBuilder.DropTable(
                name: "PNs");

            migrationBuilder.DropTable(
                name: "ResidentPaymentMethods");

            migrationBuilder.DropTable(
                name: "SpecialResponsibilities");

            migrationBuilder.DropTable(
                name: "StaffResidentStatuses");

            migrationBuilder.DropTable(
                name: "StaffShifts");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ResidentStatuses");

            migrationBuilder.DropTable(
                name: "ShiftBoards");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Residents");

            migrationBuilder.DropTable(
                name: "RiskLevels");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "GroceryDays");
        }
    }
}
