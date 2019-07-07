namespace TrainConnected.Data.Models
{
    using System.Collections.Generic;

    public interface ITrainee
    {
        string Username { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        string FullName { get; set; }

        string PhoneNumber { get; set; }

        ICollection<Workout> Workouts { get; set; }

        ICollection<Booking> Bookings { get; set; }

        ICollection<Achievement> Achievements { get; set; }
    }
}
