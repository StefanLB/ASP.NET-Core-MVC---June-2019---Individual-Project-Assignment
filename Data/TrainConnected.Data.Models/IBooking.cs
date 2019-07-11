namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Models.Enums;

    public interface IBooking
    {
        Workout Workout { get; set; }

        ApplicationUser User { get; set; }

        PaymentMethod PaymentMethod { get; set; }

        decimal Price { get; set; }
    }
}
