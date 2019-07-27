namespace TrainConnected.Data.Models
{
    using System;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Enums;

    public class Withdrawal : BaseDeletableModel<string>, IWithdrawal
    {
        public decimal Amount { get; set; }

        public string AdditionalInstructions { get; set; }

        public string TrainConnectedUserId { get; set; }
        public TrainConnectedUser TrainConnectedUser { get; set; }

        public StatusCode Status { get; set; }

        public string ProcessedByUserId { get; set; }
        public TrainConnectedUser ProcessedByUser { get; set; }

        public string ResolutionNotes { get; set; }

        public DateTime? CompletedOn { get; set; }
    }
}
