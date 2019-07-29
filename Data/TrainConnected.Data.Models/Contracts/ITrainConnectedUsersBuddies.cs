namespace TrainConnected.Data.Models.Contracts
{
    using System;

    public interface ITrainConnectedUsersBuddies
    {
        string TrainConnectedUserId { get; set; }
        TrainConnectedUser TrainConnectedUser { get; set; }

        string TrainConnectedBuddyId { get; set; }
        TrainConnectedUser TrainConnectedBuddy { get; set; }

        DateTime AddedOn { get; set; }
    }
}
