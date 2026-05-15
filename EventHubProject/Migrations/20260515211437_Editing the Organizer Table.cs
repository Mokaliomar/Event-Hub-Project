using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHubProject.Migrations
{
    /// <inheritdoc />
    public partial class EditingtheOrganizerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Organizer");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Organizer");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Organizer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Organizer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Organizer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Organizer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
