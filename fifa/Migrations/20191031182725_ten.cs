using Microsoft.EntityFrameworkCore.Migrations;

namespace fifa.Migrations
{
    public partial class ten : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuestLogo",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeLogo",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestLogo",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeLogo",
                table: "Games");
        }
    }
}
