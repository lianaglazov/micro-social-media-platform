using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class jdsf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_ApplicationUserId",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_ApplicationUserId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Friends");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Friends",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friends_ApplicationUserId",
                table: "Friends",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_ApplicationUserId",
                table: "Friends",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
