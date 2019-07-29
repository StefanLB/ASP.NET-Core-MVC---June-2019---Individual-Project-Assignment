namespace TrainConnected.Web.ViewModels.Bookings
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BookingsAllViewModel : IMapFrom<Booking>
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.Booking.PaymentMethodNameDisplay)]
        public string PaymentMethodName { get; set; }

        [Display(Name = ModelConstants.Booking.PaymentMethodPIANameDisplay)]
        public bool PaymentMethodPaymentInAdvance { get; set; }

        public decimal Price { get; set; }

        [Display(Name = ModelConstants.Booking.CreatedOnNameDisplay)]
        public DateTime CreatedOn { get; set; }

        public string WorkoutActivityIcon { get; set; }

        [Display(Name = ModelConstants.Booking.ActivityNameDisplay)]
        public string WorkoutActivityName { get; set; }

        [Display(Name = ModelConstants.Booking.CoachNameDisplay)]
        public string WorkoutCoachUserName { get; set; }

        [Display(Name = ModelConstants.Booking.TimeNameDisplay)]
        public DateTime WorkoutTime { get; set; }

        [Display(Name = ModelConstants.Booking.LocationNameDisplay)]
        public string WorkoutLocation { get; set; }

        [Display(Name = ModelConstants.Booking.DurationNameDisplay)]
        public int WorkoutDuration { get; set; }

        [Display(Name = ModelConstants.Booking.NotesNameDisplay)]
        public string WorkoutNotes { get; set; }
    }
}
