namespace TrainConnected.Web.ViewModels.Workouts
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutsAllViewModel : IMapFrom<Workout>
    {
        public string Id { get; set; }

        [Display(Name = "Activity")]
        public string ActivityName { get; set; }

        [Display(Name = "Coach")]
        public string CoachUserName { get; set; }

        public DateTime Time { get; set; }

        public string Location { get; set; }

        public int Duration { get; set; }

        public decimal Price { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Currently Signed Up")]
        public int CurrentlySignedUp { get; set; }

        [Display(Name = "Max Participants")]
        public int MaxParticipants { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }
}
