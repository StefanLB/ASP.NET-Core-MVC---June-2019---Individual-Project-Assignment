namespace TrainConnected.Web.ViewModels.Bookings
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BookingsAllViewModel : IMapFrom<Booking>
    {
        public string Id { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public decimal Price { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Activity")]
        public string WorkoutActivityName { get; set; }

        [Display(Name = "Coach")]
        public string WorkoutCoachUserName { get; set; }

        [Display(Name = "Workout Time")]
        public DateTime WorkoutTime { get; set; }

        [Display(Name = "Location")]
        public string WorkoutLocation { get; set; }

        [Display(Name = "Duration")]
        public int WorkoutDuration { get; set; }

        [Display(Name = "Notes")]
        public string WorkoutNotes { get; set; }
    }
}
