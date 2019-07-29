namespace TrainConnected.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class Workout : BaseDeletableModel<string>, IWorkout
    {
        public Workout()
        {
            this.Bookings = new HashSet<Booking>();
            this.Users = new HashSet<TrainConnectedUsersWorkouts>();
            this.PaymentMethods = new HashSet<WorkoutsPaymentMethods>();
        }

        [Required]
        public string ActivityId { get; set; }

        public WorkoutActivity Activity { get; set; }

        [Required]
        public string CoachId { get; set; }

        public TrainConnectedUser Coach { get; set; }

        [Required]
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

        public ICollection<WorkoutsPaymentMethods> PaymentMethods { get; set; }

        public string Notes { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public int CurrentlySignedUp { get => this.Bookings.Count; }

        [Required]
        [Range(ModelConstants.Workout.ParticipantsMin, ModelConstants.Workout.ParticipantsMax, ErrorMessage = ModelConstants.Workout.ParticipantsRangeError)]
        public int MaxParticipants { get; set; }

        public ICollection<TrainConnectedUsersWorkouts> Users { get; set; }
    }
}
