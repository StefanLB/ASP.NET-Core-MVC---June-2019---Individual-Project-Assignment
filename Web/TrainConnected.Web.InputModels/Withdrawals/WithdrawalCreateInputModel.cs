namespace TrainConnected.Web.InputModels.Withdrawals
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WithdrawalCreateInputModel : IMapFrom<Withdrawal>
    {
        // TODO: Add validation for the amount to be less than the user's current balance
        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Withdrawal Request Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Additional Instructions")]
        public string AdditionalInstructions { get; set; }
    }
}
