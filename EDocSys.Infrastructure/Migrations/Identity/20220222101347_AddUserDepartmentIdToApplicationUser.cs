using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations.Identity
{
    public partial class AddUserDepartmentIdToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserDepartmentId",
                schema: "Identity",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDepartmentId",
                schema: "Identity",
                table: "Users");
        }
    }
}
