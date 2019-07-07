namespace TrainConnected.Data.Models
{
    public interface IWithdrawal
    {
        decimal Amount { get; set; }

        Coach CreatedBy { get; set; }
    }
}
