using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class blablablablablabla0205 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AppCustomers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomers_ApplicationUserId",
                table: "AppCustomers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCustomers_AspNetUsers_ApplicationUserId",
                table: "AppCustomers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCustomers_AspNetUsers_ApplicationUserId",
                table: "AppCustomers");

            migrationBuilder.DropIndex(
                name: "IX_AppCustomers_ApplicationUserId",
                table: "AppCustomers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AppCustomers");
        }
    }
}
