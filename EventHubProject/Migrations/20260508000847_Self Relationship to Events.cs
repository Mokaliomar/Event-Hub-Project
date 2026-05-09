using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHubProject.Migrations
{
    /// <inheritdoc />
    public partial class SelfRelationshiptoEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Event_SessionId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Event",
                newName: "AnnualConferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_SessionId",
                table: "Event",
                newName: "IX_Event_AnnualConferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Event_AnnualConferenceId",
                table: "Event",
                column: "AnnualConferenceId",
                principalTable: "Event",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Event_AnnualConferenceId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "AnnualConferenceId",
                table: "Event",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_AnnualConferenceId",
                table: "Event",
                newName: "IX_Event_SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Event_SessionId",
                table: "Event",
                column: "SessionId",
                principalTable: "Event",
                principalColumn: "Id");
        }
    }
}
