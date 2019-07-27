namespace TrainConnected.Web.InputModels.Withdrawals
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WithdrawalProcessInputModel : IMapFrom<Withdrawal>
    {
        [Display(Name = "Transaction Id")]
        public string Id { get; set; }

        public decimal Amount { get; set; }

        [Display(Name = "Additional Instructions")]
        public string AdditionalInstructions { get; set; }

        [Display(Name ="User")]
        public string TrainConnectedUserUserName { get; set; }

        [Display(Name = "User Id")]
        public string TrainConnectedUserId { get; set; }

        public bool Status { get; set; }

        [Display(Name = "Resolution Notes")]
        public string ResolutionNotes { get; set; }
    }
}
