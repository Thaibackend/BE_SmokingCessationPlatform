using Microsoft.EntityFrameworkCore.Migrations;
using System;

public partial class UpdatePackageAndOrderStructure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. Xóa foreign key cũ của Package
        migrationBuilder.DropForeignKey(
            name: "FK_Packages_Users_CreatedBy",
            table: "Packages");

        // 2. Xóa column CreatedBy
        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Packages");

        // 3. Thêm AssignedCoachId vào Package
        migrationBuilder.AddColumn<int>(
            name: "AssignedCoachId",
            table: "Packages",
            type: "int",
            nullable: true);

        // 4. Thêm foreign key mới cho AssignedCoachId
        migrationBuilder.AddForeignKey(
            name: "FK_Packages_Users_AssignedCoachId",
            table: "Packages",
            column: "AssignedCoachId",
            principalTable: "Users",
            principalColumn: "UserId",
            onDelete: ReferentialAction.Restrict);

        // 5. Thêm columns cho Order
        migrationBuilder.AddColumn<decimal>(
            name: "CoachCommission",
            table: "Orders",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<bool>(
            name: "IsCommissionPaid",
            table: "Orders",
            type: "bit",
            nullable: false,
            defaultValue: false);

        // 6. Tạo bảng CoachCommission
        migrationBuilder.CreateTable(
            name: "CoachCommissions",
            columns: table => new
            {
                CommissionId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CoachId = table.Column<int>(type: "int", nullable: false),
                OrderId = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PaidDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CoachCommissions", x => x.CommissionId);
                table.ForeignKey(
                    name: "FK_CoachCommissions_Users_CoachId",
                    column: x => x.CoachId,
                    principalTable: "Users",
                    principalColumn: "UserId",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CoachCommissions_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "OrderId",
                    onDelete: ReferentialAction.Restrict);
            });

        // 7. Tạo indexes cho CoachCommission
        migrationBuilder.CreateIndex(
            name: "IX_CoachCommissions_CoachId",
            table: "CoachCommissions",
            column: "CoachId");

        migrationBuilder.CreateIndex(
            name: "IX_CoachCommissions_OrderId",
            table: "CoachCommissions",
            column: "OrderId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Rollback all changes
        migrationBuilder.DropTable(
            name: "CoachCommissions");

        migrationBuilder.DropForeignKey(
            name: "FK_Packages_Users_AssignedCoachId",
            table: "Packages");

        migrationBuilder.DropColumn(
            name: "AssignedCoachId",
            table: "Packages");

        migrationBuilder.DropColumn(
            name: "CoachCommission",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "IsCommissionPaid",
            table: "Orders");

        migrationBuilder.AddColumn<int>(
            name: "CreatedBy",
            table: "Packages",
            type: "int",
            nullable: false);

        migrationBuilder.AddForeignKey(
            name: "FK_Packages_Users_CreatedBy",
            table: "Packages",
            column: "CreatedBy",
            principalTable: "Users",
            principalColumn: "UserId",
            onDelete: ReferentialAction.Restrict);
    }
} 