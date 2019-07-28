namespace TrainConnected.Web.InputModels.Bookings
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BookingCreateInputModel : IMapFrom<Booking>
    {
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string WorkoutId { get; set; }
    }
}
