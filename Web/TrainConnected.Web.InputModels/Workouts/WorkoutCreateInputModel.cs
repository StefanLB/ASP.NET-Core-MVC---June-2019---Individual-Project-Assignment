namespace TrainConnected.Web.InputModels.Workouts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.PaymentMethods;

    public class WorkoutCreateInputModel : IMapFrom<Workout>
    {
        public WorkoutCreateInputModel()
        {
            this.PaymentMethods = new HashSet<string>();
        }

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
        [Display(Name ="Accepted Payment Methods")]
        public ICollection<string> PaymentMethods { get; set; }

        [Required]
        public string Notes { get; set; }

        // TODO: Add validation - cannot be a negative number
        [Required]
        public int MaxParticipants { get; set; }
    }
}
