using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class blablablablabla0205 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCustomers_AspNetUsers_ApplicationUserId1",
                table: "AppCustomers");

            migrationBuilder.DropIndex(
                name: "IX_AppCustomers_ApplicationUserId1",
                table: "AppCustomers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AppCustomers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "AppCustomers");

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "AppOrders",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppOrders");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "AppCustomers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "AppCustomers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomers_ApplicationUserId1",
                table: "AppCustomers",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCustomers_AspNetUsers_ApplicationUserId1",
                table: "AppCustomers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
