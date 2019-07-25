using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainConnected.Data.Migrations
{
    public partial class workoutActivitiesPicturePropertyChangedToActivityIcon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "WorkoutActivities",
                newName: "ActivityIcon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActivityIcon",
                table: "WorkoutActivities",
                newName: "Picture");
        }
    }
}
