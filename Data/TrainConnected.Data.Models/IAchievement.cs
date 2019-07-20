namespace TrainConnected.Data.Models
{
    using System;

    public interface IAchievement
    {
        string Name { get; set; }

        DateTime AchievedOn { get; set; }

        string Description { get; set; }

        string TrainConnectedUserId { get; set; }
        TrainConnectedUser TrainConnectedUser { get; set; }

    }
}
