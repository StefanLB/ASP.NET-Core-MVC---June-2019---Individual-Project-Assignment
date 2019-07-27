using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class WithdrawalFinalizedOnRenamedToCompletedOnAndMadeNullale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalizedOn",
                table: "Withdrawals");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "Withdrawals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "Withdrawals");

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalizedOn",
                table: "Withdrawals",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
