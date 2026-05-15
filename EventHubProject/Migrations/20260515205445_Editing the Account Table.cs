using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHubProject.Migrations
{
    /// <inheritdoc />
    public partial class EditingtheAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Account");
        }
    }
}
