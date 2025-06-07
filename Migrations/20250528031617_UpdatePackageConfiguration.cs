using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmokingQuitSupportAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackageConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Users_CreatorUserId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CreatorUserId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Packages");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Packages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Packages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CreatedBy",
                table: "Packages",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Users_CreatedBy",
                table: "Packages",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Users_CreatedBy",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CreatedBy",
                table: "Packages");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Packages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "CreatorUserId",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CreatorUserId",
                table: "Packages",
                column: "CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Users_CreatorUserId",
                table: "Packages",
                column: "CreatorUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
