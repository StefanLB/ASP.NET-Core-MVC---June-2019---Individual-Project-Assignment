namespace TrainConnected.Data.Models
{
    using System.Collections.Generic;

    public interface ITrainConnectedUser
    {
        string UserName { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string PhoneNumber { get; set; }

        decimal Balance { get; set; }

        ICollection<Certificate> Certificates { get; set; }

        ICollection<TrainConnectedUsersWorkouts> Workouts { get; set; }

        ICollection<Booking> Bookings { get; set; }

        ICollection<Achievement> Achievements { get; set; }

        ICollection<Withdrawal> Withdrawals { get; set; }

        ICollection<TrainConnectedUser> Buddies { get; set; }
    }
}
