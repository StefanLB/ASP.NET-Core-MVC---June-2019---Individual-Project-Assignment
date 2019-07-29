namespace TrainConnected.Web.ViewModels.Workouts
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutsAllViewModel : IMapFrom<Workout>
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.Workout.ActivityNameDisplay)]
        public string ActivityName { get; set; }

        public string ActivityIcon { get; set; }

        [Display(Name = ModelConstants.Workout.CoachNameDisplay)]
        public string CoachUserName { get; set; }

        public DateTime Time { get; set; }

        public string Location { get; set; }

        public int Duration { get; set; }

        public decimal Price { get; set; }

        public string Notes { get; set; }

        [Display(Name = ModelConstants.Workout.BookingsCountNameDisplay)]
        public int BookingsCount { get; set; }

        [Display(Name = ModelConstants.Workout.MaxParticipantsNameDisplay)]
        public int MaxParticipants { get; set; }

        [Display(Name = ModelConstants.Workout.CreatedOnNameDisplay)]
        public DateTime CreatedOn { get; set; }
    }
}
