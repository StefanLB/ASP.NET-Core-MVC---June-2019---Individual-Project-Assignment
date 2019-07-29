namespace TrainConnected.Web.InputModels.Withdrawals
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WithdrawalProcessInputModel : IMapFrom<Withdrawal>
    {
        [Required]
        [Display(Name = ModelConstants.Withdrawal.IdNameDisplay)]
        public string Id { get; set; }

        [Required]
        [Range(typeof(decimal), ModelConstants.PriceMin, ModelConstants.PriceMax, ErrorMessage = ModelConstants.PriceRangeError)]
        [Display(Name = ModelConstants.Withdrawal.AmountNameDisplay)]
        public decimal Amount { get; set; }
        
        [Display(Name = ModelConstants.Withdrawal.AdditionalInstructionsNameDisplay)]
        public string AdditionalInstructions { get; set; }

        [Required]
        [Display(Name = ModelConstants.Withdrawal.UserNameDisplay)]
        public string TrainConnectedUserUserName { get; set; }

        [Required]
        [Display(Name = ModelConstants.Withdrawal.UserIdNameDisplay)]
        public string TrainConnectedUserId { get; set; }

        [Required]
        public bool Status { get; set; }

        [Display(Name = ModelConstants.Withdrawal.ResolutionNotesNameDisplay)]
        public string ResolutionNotes { get; set; }
    }
}
