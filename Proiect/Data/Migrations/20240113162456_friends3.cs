using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class friends3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserFriend");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Friends",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUrmaritId",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUrmaritorId",
                table: "Friends",
                type: "nvarchar(max)",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "UserUrmaritId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "UserUrmaritorId",
                table: "Friends");

            migrationBuilder.CreateTable(
                name: "ApplicationUserFriend",
                columns: table => new
                {
                    FriendsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserFriend", x => new { x.FriendsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserFriend_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserFriend_Friends_FriendsId",
                        column: x => x.FriendsId,
                        principalTable: "Friends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserFriend_UsersId",
                table: "ApplicationUserFriend",
                column: "UsersId");
        }
    }
}
