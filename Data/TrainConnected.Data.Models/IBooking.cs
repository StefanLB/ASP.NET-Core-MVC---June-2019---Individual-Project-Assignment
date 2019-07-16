namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Models.Enums;

    public interface IBooking
    {
        TrainConnectedUser TrainConnectedUser { get; set; }

        PaymentMethod PaymentMethod { get; set; }

        decimal Price { get; set; }
    }
}
