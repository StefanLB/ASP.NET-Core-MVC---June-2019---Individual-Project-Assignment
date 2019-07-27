namespace TrainConnected.Data.Models
{
    using System;
    using TrainConnected.Data.Models.Enums;

    public interface IWithdrawal
    {
        decimal Amount { get; set; }

        string AdditionalInstructions { get; set; }

        string TrainConnectedUserId { get; set; }
        TrainConnectedUser TrainConnectedUser { get; set; }

        StatusCode Status { get; set; }

        string ProcessedByUserId { get; set; }
        TrainConnectedUser ProcessedByUser { get; set; }

        string ResolutionNotes { get; set; }

        DateTime FinalizedOn { get; set; }
    }
}
