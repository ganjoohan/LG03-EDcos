using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class addcolumnWI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "WorkInstructions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Concurred1",
                table: "WorkInstructions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Concurred2",
                table: "WorkInstructions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedBy",
                table: "WorkInstructions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreparedByDate",
                table: "WorkInstructions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedByPosition",
                table: "WorkInstructions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcedureStatusView",
                table: "WorkInstructions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "hasSOP",
                table: "WorkInstructions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "Concurred1",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "Concurred2",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "PreparedBy",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "PreparedByDate",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "PreparedByPosition",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "ProcedureStatusView",
                table: "WorkInstructions");

            migrationBuilder.DropColumn(
                name: "hasSOP",
                table: "WorkInstructions");
        }
    }
}
