namespace TrainConnected.Web.InputModels.Withdrawals
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WithdrawalCreateInputModel : IMapFrom<Withdrawal>
    {
        // TODO: Add validation for the amount to be less than the user's current balance
        [Required]
        [Range(typeof(decimal), ModelConstants.PriceMin, ModelConstants.PriceMax, ErrorMessage = ModelConstants.PriceRangeError)]
        [Display(Name = ModelConstants.Withdrawal.AmountNameDisplay)]
        public decimal Amount { get; set; }

        [Display(Name = ModelConstants.Withdrawal.AdditionalInstructionsNameDisplay)]
        public string AdditionalInstructions { get; set; }
    }
}
