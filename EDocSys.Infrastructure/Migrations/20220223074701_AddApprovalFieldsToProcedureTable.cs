using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class AddApprovalFieldsToProcedureTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Procedures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Concurred1",
                table: "Procedures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Concurred2",
                table: "Procedures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedBy",
                table: "Procedures",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "Concurred1",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "Concurred2",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "PreparedBy",
                table: "Procedures");
        }
    }
}
