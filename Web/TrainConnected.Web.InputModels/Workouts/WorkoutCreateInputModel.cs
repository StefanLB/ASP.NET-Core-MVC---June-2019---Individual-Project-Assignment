namespace TrainConnected.Web.InputModels.Workouts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Common.Attributes;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutCreateInputModel : IMapFrom<Workout>
    {
        public WorkoutCreateInputModel()
        {
            this.PaymentMethods = new HashSet<string>();
        }

        [Required]
        public string Activity { get; set; }

        [Required]
        [GreaterThanOrEqualAttribute(nameof(DateTimeNow), ErrorMessage = ModelConstants.Workout.TimeError)]
        public DateTime Time { get; set; }

        [Required]
        [StringLength(ModelConstants.Workout.LocationNameMaxLength, MinimumLength = ModelConstants.Workout.LocationNameMinLength, ErrorMessage = ModelConstants.Workout.LocationRangeError)]
        public string Location { get; set; }

        [Required]
        [Range(ModelConstants.Workout.DurationMin, ModelConstants.Workout.DurationMax, ErrorMessage = ModelConstants.Workout.DurationRangeError)]
        public int Duration { get; set; }

        [Required]
        [Range(typeof(decimal), ModelConstants.PriceMin, ModelConstants.PriceMax, ErrorMessage = ModelConstants.PriceRangeError)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = ModelConstants.Workout.PaymentMethodsNameDisplay)]
        public ICollection<string> PaymentMethods { get; set; }

        [StringLength(ModelConstants.Workout.NotesMaxLength, ErrorMessage = ModelConstants.Workout.NotesLengthError)]
        public string Notes { get; set; }

        [Required]
        [Range(ModelConstants.Workout.ParticipantsMin, ModelConstants.Workout.ParticipantsMax, ErrorMessage = ModelConstants.Workout.ParticipantsRangeError)]
        public int MaxParticipants { get; set; }

        public DateTime DateTimeNow
        {
            get { return DateTime.Now; }
        }
    }
}
