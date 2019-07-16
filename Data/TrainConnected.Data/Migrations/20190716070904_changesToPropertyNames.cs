using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class changesToPropertyNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Certificates");

            migrationBuilder.AlterColumn<string>(
                name: "IssuedTo",
                table: "Certificates",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IssuedBy",
                table: "Certificates",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Certificates",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Activity",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                table: "Certificates");

            migrationBuilder.AlterColumn<string>(
                name: "IssuedTo",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IssuedBy",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);
        }
    }
}
