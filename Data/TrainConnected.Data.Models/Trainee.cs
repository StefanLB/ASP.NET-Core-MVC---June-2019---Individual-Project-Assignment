﻿namespace TrainConnected.Data.Models
{
    using System.Collections.Generic;
    using TrainConnected.Data.Common.Models;

    public class Trainee : BaseDeletableModel<string>, ITrainee
    {
        public Trainee()
        {
            this.Workouts = new HashSet<Workout>();

            this.Bookings = new HashSet<Booking>();

            this.Achievements = new HashSet<Achievement>();
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public ICollection<Workout> Workouts { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public ICollection<Achievement> Achievements { get; set; }
    }
}
