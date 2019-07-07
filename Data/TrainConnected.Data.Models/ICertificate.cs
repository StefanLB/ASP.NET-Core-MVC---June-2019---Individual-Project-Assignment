namespace TrainConnected.Data.Models
{
    using System;
    using TrainConnected.Data.Models.Enums;

    public interface ICertificate
    {
        WorkoutCategory Category { get; set; }

        string IssuedBy { get; set; }

        string IssuedTo { get; set; }

        DateTime IssuedOn { get; set; }

        DateTime? ExpiresOn { get; set; }

        string Description { get; set; }
    }
}
