namespace TrainConnected.Data.Models
{
    public interface IWithdrawal
    {
        decimal Amount { get; set; }

        string TrainConnectedUserId { get; set; }
        TrainConnectedUser TrainConnectedUser { get; set; }
    }
}
