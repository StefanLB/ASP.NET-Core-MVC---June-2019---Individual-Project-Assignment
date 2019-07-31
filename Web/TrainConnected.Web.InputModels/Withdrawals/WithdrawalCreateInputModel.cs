namespace TrainConnected.Web.InputModels.Withdrawals
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Common.Attributes;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WithdrawalCreateInputModel : IMapFrom<Withdrawal>
    {
        [Required]
        [Display(Name = ModelConstants.Withdrawal.AmountNameDisplay)]
        [Range(typeof(decimal), ModelConstants.Withdrawal.AmountMin, ModelConstants.PriceMax, ErrorMessage = ModelConstants.Withdrawal.NegativeAmountError)]
        [LessThanOrEqual(nameof(WithdrawableAmount), ErrorMessage = ModelConstants.Withdrawal.AmountError)]
        public decimal Amount { get; set; }

        [Display(Name = ModelConstants.Withdrawal.AdditionalInstructionsNameDisplay)]
        [StringLength(ModelConstants.Withdrawal.AdditionalInstructionsMaxLength, ErrorMessage = ModelConstants.Withdrawal.AdditionalInstructionsLengthError)]
        public string AdditionalInstructions { get; set; }

        public decimal WithdrawableAmount { get; set; }
    }
}
