using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class smallChangesToModelsMostlyTCUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInstructions",
                table: "Withdrawals",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInstructions",
                table: "Withdrawals",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
