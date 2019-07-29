namespace TrainConnected.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;
    using TrainConnected.Data.Models.Enums;

    public class Withdrawal : BaseDeletableModel<string>, IWithdrawal
    {
        [Required]
        [Range(typeof(decimal), ModelConstants.PriceMin, ModelConstants.PriceMax, ErrorMessage = ModelConstants.PriceRangeError)]
        public decimal Amount { get; set; }

        [StringLength(ModelConstants.Withdrawal.AdditionalInstructionsMaxLength, ErrorMessage = ModelConstants.Withdrawal.AdditionalInstructionsLengthError)]
        public string AdditionalInstructions { get; set; }

        [Required]
        public string TrainConnectedUserId { get; set; }

        public TrainConnectedUser TrainConnectedUser { get; set; }

        [Required]
        public StatusCode Status { get; set; }

        public string ProcessedByUserId { get; set; }

        public TrainConnectedUser ProcessedByUser { get; set; }

        public string ResolutionNotes { get; set; }

        public DateTime? CompletedOn { get; set; }
    }
}
