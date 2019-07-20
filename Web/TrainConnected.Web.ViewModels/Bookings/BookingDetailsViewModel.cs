namespace TrainConnected.Web.ViewModels.Bookings
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BookingDetailsViewModel : IMapFrom<Booking>
    {
        public string Id { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public decimal Price { get; set; }

        [Display(Name ="Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Activity")]
        public string WorkoutActivityName { get; set; }

        [Display(Name = "Coach")]
        public string WorkoutCoachUserName { get; set; }

        [Display(Name = "Time")]
        public DateTime WorkoutTime { get; set; }

        [Display(Name = "Location")]
        public string WorkoutLocation { get; set; }

        [Display(Name = "Duration")]
        public int WorkoutDuration { get; set; }

        [Display(Name ="Notes")]
        public string WorkoutNotes { get; set; }

        [Display(Name = "Currently Signed Up")]
        public int WorkoutBookingsCount { get; set; }

        [Display(Name = "Max Participants")]
        public int WorkoutMaxParticipants { get; set; }
    }
}
