using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class AddReferenceDepartmentToUserApproverTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserApprovers_DepartmentId",
                table: "UserApprovers",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApprovers_Department_DepartmentId",
                table: "UserApprovers",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApprovers_Department_DepartmentId",
                table: "UserApprovers");

            migrationBuilder.DropIndex(
                name: "IX_UserApprovers_DepartmentId",
                table: "UserApprovers");
        }
    }
}
