namespace TrainConnected.Web.InputModels.Workouts
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutCreateInputModel : IMapFrom<Workout>
    {
        [Required]
        public string Activity { get; set; }

        // TODO: Add validation - time has to be in the future
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public string Location { get; set; }

        // TODO: Add validation - cannot be a negative number
        [Required]
        public int Duration { get; set; }

        // TODO: Add validation - cannot be a negative number
        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Notes { get; set; }

        // TODO: Add validation - cannot be a negative number
        [Required]
        public int MaxParticipants { get; set; }
    }
}
