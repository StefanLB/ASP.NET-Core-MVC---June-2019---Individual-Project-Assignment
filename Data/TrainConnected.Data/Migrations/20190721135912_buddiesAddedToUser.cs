using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class buddiesAddedToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrainConnectedUserId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TrainConnectedUserId",
                table: "AspNetUsers",
                column: "TrainConnectedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_TrainConnectedUserId",
                table: "AspNetUsers",
                column: "TrainConnectedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_TrainConnectedUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TrainConnectedUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TrainConnectedUserId",
                table: "AspNetUsers");
        }
    }
}
