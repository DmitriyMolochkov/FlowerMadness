using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class blablabla0205 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AspNetUsers_CashierId",
                table: "AppOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppProducts_ParentId",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_ParentId",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_CashierId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "CashierId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "AppProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CashierId",
                table: "AppOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_ParentId",
                table: "AppProducts",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_CashierId",
                table: "AppOrders",
                column: "CashierId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AspNetUsers_CashierId",
                table: "AppOrders",
                column: "CashierId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppProducts_ParentId",
                table: "AppProducts",
                column: "ParentId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
