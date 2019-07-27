using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class updatedWithdrawalsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInstructions",
                table: "Withdrawals",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalizedOn",
                table: "Withdrawals",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProcessedByUserId",
                table: "Withdrawals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "Withdrawals",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Withdrawals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawals_ProcessedByUserId",
                table: "Withdrawals",
                column: "ProcessedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Withdrawals_AspNetUsers_ProcessedByUserId",
                table: "Withdrawals",
                column: "ProcessedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Withdrawals_AspNetUsers_ProcessedByUserId",
                table: "Withdrawals");

            migrationBuilder.DropIndex(
                name: "IX_Withdrawals_ProcessedByUserId",
                table: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "AdditionalInstructions",
                table: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "FinalizedOn",
                table: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "ProcessedByUserId",
                table: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Withdrawals");
        }
    }
}
