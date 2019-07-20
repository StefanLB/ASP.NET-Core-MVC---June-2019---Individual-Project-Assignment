using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class updatingAchievementEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstAchievedOn",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "LastAchievedOn",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "TimesAchieved",
                table: "Achievements");

            migrationBuilder.AddColumn<DateTime>(
                name: "AchievedOn",
                table: "Achievements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AchievedOn",
                table: "Achievements");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstAchievedOn",
                table: "Achievements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAchievedOn",
                table: "Achievements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimesAchieved",
                table: "Achievements",
                nullable: false,
                defaultValue: 0);
        }
    }
}
