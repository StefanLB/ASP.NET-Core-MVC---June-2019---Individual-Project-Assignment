namespace TrainConnected.Web.ViewModels.Workouts
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutsHomeViewModel : IMapFrom<Workout>
    {
        public string Id { get; set; }

        [Display(Name = "Activity")]
        public string ActivityName { get; set; }

        [Display(Name = "Coach")]
        public string CoachUserName { get; set; }

        public DateTime Time { get; set; }

        public string Location { get; set; }

        public int Duration { get; set; }

        [Display(Name = "Currently Signed Up")]
        public int BookingsCount { get; set; }

        [Display(Name = "Max Participants")]
        public int MaxParticipants { get; set; }
    }
}
