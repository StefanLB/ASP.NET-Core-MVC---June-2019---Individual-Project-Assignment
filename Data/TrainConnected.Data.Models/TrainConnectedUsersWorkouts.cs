namespace TrainConnected.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models.Contracts;

    public class TrainConnectedUsersWorkouts : ITrainConnectedUsersWorkouts
    {
        [Required]
        public string TrainConnectedUserId { get; set; }

        public TrainConnectedUser TrainConnectedUser { get; set; }

        [Required]
        public string WorkoutId { get; set; }

        public Workout Workout { get; set; }
    }
}
