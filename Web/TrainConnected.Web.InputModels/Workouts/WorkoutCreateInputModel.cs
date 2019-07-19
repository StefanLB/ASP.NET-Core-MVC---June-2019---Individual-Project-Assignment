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

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Notes { get; set; }

        [Required]
        public int MaxParticipants { get; set; }
    }
}
