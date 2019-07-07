﻿namespace TrainConnected.Data.Models
{
    using System;
    using System.Collections.Generic;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Enums;

    public class Workout : BaseDeletableModel<string>, IWorkout
    {
        public Workout()
        {
            this.Bookings = new HashSet<Booking>();
        }

        public WorkoutCategory Category { get; set; }

        public Coach Coach { get; set; }

        public DateTime Time { get; set; }

        public string Location { get; set; }

        public int Duration { get; set; }

        public decimal Price { get; set; }

        public string Notes { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public int CurrentlySignedUp { get => this.Bookings.Count; }

        public int MaxParticipants { get; set; }
    }
}