namespace TrainConnected.Data.Models.Contracts
{

    public interface IBooking
    {
        string TrainConnectedUserId { get; set; }
        TrainConnectedUser TrainConnectedUser { get; set; }

        string PaymentMethodId { get; set; }
        PaymentMethod PaymentMethod { get; set; }

        decimal Price { get; set; }

        string WorkoutId { get; set; }
        Workout Workout { get; set; }
    }
}
