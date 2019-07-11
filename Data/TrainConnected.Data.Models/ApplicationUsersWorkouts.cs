namespace TrainConnected.Data.Models
{
    public class ApplicationUsersWorkouts : IApplicationUsersWorkouts
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string WorkoutId { get; set; }

        public Workout Workout { get; set; }
    }
}
