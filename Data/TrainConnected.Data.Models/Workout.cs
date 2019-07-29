namespace TrainConnected.Data.Models
{
    using System;
    using System.Collections.Generic;
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

        public string ActivityId { get; set; }
        public WorkoutActivity Activity { get; set; }

        public string CoachId { get; set; }
        public TrainConnectedUser Coach { get; set; }

        public DateTime Time { get; set; }

        public string Location { get; set; }

        public int Duration { get; set; }

        public decimal Price { get; set; }

        public ICollection<WorkoutsPaymentMethods> PaymentMethods { get; set; }

        public string Notes { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public int CurrentlySignedUp { get => this.Bookings.Count; }

        public int MaxParticipants { get; set; }

        public ICollection<TrainConnectedUsersWorkouts> Users { get; set; }
    }
}
