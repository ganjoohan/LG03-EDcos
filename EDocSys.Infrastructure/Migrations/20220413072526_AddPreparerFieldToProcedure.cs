using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class AddPreparerFieldToProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PreparedByDate",
                table: "Procedures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedByPosition",
                table: "Procedures",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreparedByDate",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "PreparedByPosition",
                table: "Procedures");
        }
    }
}
