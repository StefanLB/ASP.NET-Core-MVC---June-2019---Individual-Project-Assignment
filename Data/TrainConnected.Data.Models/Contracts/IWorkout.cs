namespace TrainConnected.Data.Models.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IWorkout
    {
        string ActivityId { get; set; }
        WorkoutActivity Activity { get; set; }

        string CoachId { get; set; }
        TrainConnectedUser Coach { get; set; }

        DateTime Time { get; set; }

        string Location { get; set; }

        int Duration { get; set; }

        decimal Price { get; set; }

        ICollection<WorkoutsPaymentMethods> PaymentMethods { get; set; }

        string Notes { get; set; }

        ICollection<Booking> Bookings { get; set; }

        int CurrentlySignedUp { get; }

        int MaxParticipants { get; set; }

        ICollection<TrainConnectedUsersWorkouts> Users { get; set; }
    }
}
