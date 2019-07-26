namespace TrainConnected.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using TrainConnected.Data.Common.Models;

    public class Booking : BaseDeletableModel<string>, IBooking
    {
        public string TrainConnectedUserId { get; set; }
        public TrainConnectedUser TrainConnectedUser { get; set; }

        public string PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public decimal Price { get; set; }

        public string WorkoutId { get; set; }
        public Workout Workout { get; set; }
    }
}
