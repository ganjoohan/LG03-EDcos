using Microsoft.EntityFrameworkCore.Migrations;

namespace EDocSys.Infrastructure.Migrations
{
    public partial class AddReferenceCompanyTableToUserApprover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserApprovers_CompanyId",
                table: "UserApprovers",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApprovers_Company_CompanyId",
                table: "UserApprovers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApprovers_Company_CompanyId",
                table: "UserApprovers");

            migrationBuilder.DropIndex(
                name: "IX_UserApprovers_CompanyId",
                table: "UserApprovers");
        }
    }
}
