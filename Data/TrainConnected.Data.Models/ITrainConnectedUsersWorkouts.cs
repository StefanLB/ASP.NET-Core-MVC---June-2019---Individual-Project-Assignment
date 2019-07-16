namespace TrainConnected.Data.Models
{
    public interface ITrainConnectedUsersWorkouts
    {
        string TrainConnectedUserId { get; set; }

        TrainConnectedUser TrainConnectedUser { get; set; }

        string WorkoutId { get; set; }

        Workout Workout { get; set; }
    }
}
