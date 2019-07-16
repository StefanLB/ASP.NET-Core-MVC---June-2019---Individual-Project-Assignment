namespace TrainConnected.Data.Models
{
    public class TrainConnectedUsersWorkouts : ITrainConnectedUsersWorkouts
    {
        public string TrainConnectedUserId { get; set; }

        public TrainConnectedUser TrainConnectedUser { get; set; }

        public string WorkoutId { get; set; }

        public Workout Workout { get; set; }
    }
}
