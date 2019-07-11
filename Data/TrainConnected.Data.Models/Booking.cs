namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Enums;

    public class Booking : BaseDeletableModel<int>, IBooking
    {
        public ApplicationUser ApplicationUser { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal Price { get; set; }
    }
}
