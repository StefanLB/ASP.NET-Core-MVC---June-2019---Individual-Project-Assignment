namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Common.Models;

    public class Withdrawal : BaseModel<string>, IWithdrawal
    {
        public decimal Amount { get; set; }

        public ApplicationUser CreatedBy { get; set; }
    }
}
