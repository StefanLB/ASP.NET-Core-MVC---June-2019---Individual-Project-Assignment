namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Models.Enums;

    public interface IBooking
    {
        string TrainConnectedUserId { get; set; }
        TrainConnectedUser TrainConnectedUser { get; set; }

        PaymentMethod PaymentMethod { get; set; }

        decimal Price { get; set; }

        string WorkoutId { get; set; }
        Workout Workout { get; set; }
    }
}
