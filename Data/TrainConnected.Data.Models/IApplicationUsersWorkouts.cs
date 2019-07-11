namespace TrainConnected.Data.Models
{
    public interface IApplicationUsersWorkouts
    {
        string UserId { get; set; }

        ApplicationUser User { get; set; }

        string WorkoutId { get; set; }

        Workout Workout { get; set; }
    }
}
