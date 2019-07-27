namespace TrainConnected.Web.ViewModels.Withdrawals
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WithdrawalsProcessingViewModel : IMapFrom<Withdrawal>
    {
        [Display(Name = "Transaction Id")]
        public string Id { get; set; }

        public decimal Amount { get; set; }

        [Display(Name = "Additional Instructions")]
        public string AdditionalInstructions { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Created By")]
        public string TrainConnectedUserUserName { get; set; }

        public string TrainConnectedUserId { get; set; }

        public string Status { get; set; }

        [Display(Name ="Resolution Notes")]
        public string ResolutionNotes { get; set; }

        [Display(Name = "Completed On")]
        public DateTime CompletedOn { get; set; }

        [Display(Name = "Completed By")]
        public string ProcessedByUserUserName { get; set; }
    }
}
