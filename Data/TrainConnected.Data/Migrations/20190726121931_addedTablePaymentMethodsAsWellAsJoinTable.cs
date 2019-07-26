using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class addedTablePaymentMethodsAsWellAsJoinTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                table: "Bookings",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PaymentInAdvance = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutsPaymentMethods",
                columns: table => new
                {
                    WorkoutId = table.Column<string>(nullable: false),
                    PaymentMethodId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutsPaymentMethods", x => new { x.WorkoutId, x.PaymentMethodId });
                    table.ForeignKey(
                        name: "FK_WorkoutsPaymentMethods_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutsPaymentMethods_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PaymentMethodId",
                table: "Bookings",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_IsDeleted",
                table: "PaymentMethods",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutsPaymentMethods_PaymentMethodId",
                table: "WorkoutsPaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_PaymentMethods_PaymentMethodId",
                table: "Bookings",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_PaymentMethods_PaymentMethodId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "WorkoutsPaymentMethods");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PaymentMethodId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Bookings",
                nullable: false,
                defaultValue: 0);
        }
    }
}
