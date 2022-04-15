using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class AddPreparerFieldToSOP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreparedBy",
                table: "StandardOperatingPractices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreparedByDate",
                table: "StandardOperatingPractices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedByPosition",
                table: "StandardOperatingPractices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreparedBy",
                table: "StandardOperatingPractices");

            migrationBuilder.DropColumn(
                name: "PreparedByDate",
                table: "StandardOperatingPractices");

            migrationBuilder.DropColumn(
                name: "PreparedByPosition",
                table: "StandardOperatingPractices");
        }
    }
}
