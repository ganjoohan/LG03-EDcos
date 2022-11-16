using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class DocumentManual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WorkInstructions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StandardOperatingPractices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Procedures",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StandardOperatingPractices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Procedures");
        }
    }
}
