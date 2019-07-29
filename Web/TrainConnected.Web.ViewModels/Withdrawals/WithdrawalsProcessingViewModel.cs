﻿namespace TrainConnected.Web.ViewModels.Withdrawals
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WithdrawalsProcessingViewModel : IMapFrom<Withdrawal>
    {
        [Display(Name = ModelConstants.Withdrawal.IdNameDisplay)]
        public string Id { get; set; }

        [Display(Name = ModelConstants.Withdrawal.AmountNameDisplay)]
        public decimal Amount { get; set; }

        [Display(Name = ModelConstants.Withdrawal.AdditionalInstructionsNameDisplay)]
        public string AdditionalInstructions { get; set; }

        [Display(Name = ModelConstants.Withdrawal.CreatedOnNameDisplay)]
        public DateTime CreatedOn { get; set; }

        [Display(Name = ModelConstants.Withdrawal.TrainConnectedUserUserNameDisplay)]
        public string TrainConnectedUserUserName { get; set; }

        public string TrainConnectedUserId { get; set; }

        public string Status { get; set; }

        [Display(Name = ModelConstants.Withdrawal.ResolutionNotesNameDisplay)]
        public string ResolutionNotes { get; set; }

        [Display(Name = ModelConstants.Withdrawal.CompletedOnNameDisplay)]
        public DateTime CompletedOn { get; set; }

        [Display(Name = ModelConstants.Withdrawal.ProcessedByUserUserNameDisplay)]
        public string ProcessedByUserUserName { get; set; }
    }
}
