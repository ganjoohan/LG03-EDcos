using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class AddApproverFieldToSOP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "StandardOperatingPractices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Concurred1",
                table: "StandardOperatingPractices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Concurred2",
                table: "StandardOperatingPractices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "StandardOperatingPractices");

            migrationBuilder.DropColumn(
                name: "Concurred1",
                table: "StandardOperatingPractices");

            migrationBuilder.DropColumn(
                name: "Concurred2",
                table: "StandardOperatingPractices");
        }
    }
}
