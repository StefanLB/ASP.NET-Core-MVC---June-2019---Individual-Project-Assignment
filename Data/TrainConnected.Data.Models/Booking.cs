namespace TrainConnected.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class Booking : BaseDeletableModel<string>, IBooking
    {
        [Required]
        public string TrainConnectedUserId { get; set; }

        public TrainConnectedUser TrainConnectedUser { get; set; }

        [Required]
        public string PaymentMethodId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Range(typeof(decimal), ModelConstants.PriceMin, ModelConstants.PriceMax, ErrorMessage = ModelConstants.PriceRangeError)]
        public decimal Price { get; set; }

        [Required]
        public string WorkoutId { get; set; }

        public Workout Workout { get; set; }
    }
}
