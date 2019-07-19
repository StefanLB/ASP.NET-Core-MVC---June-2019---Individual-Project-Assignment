namespace TrainConnected.Web.InputModels.Bookings
{
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BookingCreateInputModel : IMapFrom<Booking>
    {
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public decimal Price { get; set; }

        public string WorkoutId { get; set; }
    }
}
