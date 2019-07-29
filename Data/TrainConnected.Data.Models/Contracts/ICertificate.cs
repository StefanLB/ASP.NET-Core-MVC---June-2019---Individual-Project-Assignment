namespace TrainConnected.Data.Models.Contracts
{
    using System;

    public interface ICertificate
    {
        string ActivityId { get; set; }
        WorkoutActivity Activity { get; set; }

        string IssuedBy { get; set; }

        DateTime IssuedOn { get; set; }

        DateTime? ExpiresOn { get; set; }

        string Description { get; set; }

        string TrainConnectedUserId { get; set; }
        TrainConnectedUser TrainConnectedUser { get; set; }
    }
}
