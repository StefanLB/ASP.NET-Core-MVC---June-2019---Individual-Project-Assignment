﻿namespace TrainConnected.Data.Models
{
    using System;
    using System.Collections.Generic;
    using TrainConnected.Data.Models.Enums;

    public interface IWorkout
    {
        WorkoutCategory Category { get; set; }

        Coach Coach { get; set; }

        DateTime Time { get; set; }

        string Location { get; set; }

        int Duration { get; set; }

        decimal Price { get; set; }

        string Notes { get; set; }

        ICollection<Booking> Bookings { get; set; }

        int CurrentlySignedUp { get; }

        int MaxParticipants { get; set; }
    }
}