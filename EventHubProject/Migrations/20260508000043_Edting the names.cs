using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHubProject.Migrations
{
    /// <inheritdoc />
    public partial class Edtingthenames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Organizer_OrganizerId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Organizer_OrganizerId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfilePages_Accounts_AccountId",
                table: "ProfilePages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfilePages",
                table: "ProfilePages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "ProfilePages",
                newName: "ProfilePage");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "Account");

            migrationBuilder.RenameIndex(
                name: "IX_ProfilePages_AccountId",
                table: "ProfilePage",
                newName: "IX_ProfilePage_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_OrganizerId",
                table: "Account",
                newName: "IX_Account_OrganizerId");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizerId",
                table: "Event",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfilePage",
                table: "ProfilePage",
                column: "ProfilePageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Account",
                table: "Account",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Organizer_OrganizerId",
                table: "Account",
                column: "OrganizerId",
                principalTable: "Organizer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Organizer_OrganizerId",
                table: "Event",
                column: "OrganizerId",
                principalTable: "Organizer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfilePage_Account_AccountId",
                table: "ProfilePage",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Organizer_OrganizerId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Organizer_OrganizerId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfilePage_Account_AccountId",
                table: "ProfilePage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfilePage",
                table: "ProfilePage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Account",
                table: "Account");

            migrationBuilder.RenameTable(
                name: "ProfilePage",
                newName: "ProfilePages");

            migrationBuilder.RenameTable(
                name: "Account",
                newName: "Accounts");

            migrationBuilder.RenameIndex(
                name: "IX_ProfilePage_AccountId",
                table: "ProfilePages",
                newName: "IX_ProfilePages_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Account_OrganizerId",
                table: "Accounts",
                newName: "IX_Accounts_OrganizerId");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizerId",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfilePages",
                table: "ProfilePages",
                column: "ProfilePageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Organizer_OrganizerId",
                table: "Accounts",
                column: "OrganizerId",
                principalTable: "Organizer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Organizer_OrganizerId",
                table: "Event",
                column: "OrganizerId",
                principalTable: "Organizer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfilePages_Accounts_AccountId",
                table: "ProfilePages",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
