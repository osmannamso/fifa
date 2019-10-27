using Microsoft.EntityFrameworkCore.Migrations;

namespace fifa.Migrations
{
    public partial class six : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClubId",
                table: "Players",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClubId",
                table: "Players",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
