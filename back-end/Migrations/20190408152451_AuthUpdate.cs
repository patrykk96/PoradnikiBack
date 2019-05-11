using Microsoft.EntityFrameworkCore.Migrations;

namespace back_end.Migrations
{
    public partial class AuthUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Confirmed",
                table: "Users",
                newName: "ConfirmedAccount");

            migrationBuilder.RenameColumn(
                name: "ConfirmCode",
                table: "Users",
                newName: "VerificationCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerificationCode",
                table: "Users",
                newName: "ConfirmCode");

            migrationBuilder.RenameColumn(
                name: "ConfirmedAccount",
                table: "Users",
                newName: "Confirmed");
        }
    }
}
