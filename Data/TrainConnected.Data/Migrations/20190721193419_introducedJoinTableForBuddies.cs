namespace TrainConnected.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class introducedJoinTableForBuddies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "TrainConnectedUsersBuddies",
                columns: table => new
                {
                    TrainConnectedUserId = table.Column<string>(nullable: false),
                    TrainConnectedBuddyId = table.Column<string>(nullable: false),
                    AddedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainConnectedUsersBuddies", x => new { x.TrainConnectedUserId, x.TrainConnectedBuddyId });
                    table.ForeignKey(
                        name: "FK_TrainConnectedUsersBuddies_AspNetUsers_TrainConnectedBuddyId",
                        column: x => x.TrainConnectedBuddyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainConnectedUsersBuddies_AspNetUsers_TrainConnectedUserId",
                        column: x => x.TrainConnectedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainConnectedUsersBuddies_TrainConnectedBuddyId",
                table: "TrainConnectedUsersBuddies",
                column: "TrainConnectedBuddyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainConnectedUsersBuddies");

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
    }
}
