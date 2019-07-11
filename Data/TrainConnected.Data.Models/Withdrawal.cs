namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Common.Models;

    public class Withdrawal : BaseDeletableModel<string>, IWithdrawal
    {
        public decimal Amount { get; set; }
    }
}
