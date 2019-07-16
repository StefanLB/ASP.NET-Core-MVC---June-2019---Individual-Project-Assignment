namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Common.Models;

    public class Withdrawal : BaseDeletableModel<string>, IWithdrawal
    {
        public decimal Amount { get; set; }

        public string TrainConnectedUserId { get; set; }
        public TrainConnectedUser TrainConnectedUser { get; set; }

    }
}
