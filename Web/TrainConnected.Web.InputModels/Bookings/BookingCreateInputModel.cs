namespace TrainConnected.Web.InputModels.Bookings
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BookingCreateInputModel : IMapFrom<Booking>
    {
        [Required]
        [Display(Name = ModelConstants.Booking.PaymentMethodNameDisplay)]
        public string PaymentMethod { get; set; }

        [Required]
        [Range(typeof(decimal), ModelConstants.PriceMin, ModelConstants.PriceMax, ErrorMessage = ModelConstants.PriceRangeError)]
        public decimal Price { get; set; }

        [Required]
        public string WorkoutId { get; set; }
    }
}
