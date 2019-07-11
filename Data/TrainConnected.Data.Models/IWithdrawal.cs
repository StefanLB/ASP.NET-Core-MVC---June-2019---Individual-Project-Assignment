namespace TrainConnected.Data.Models
{
    public interface IWithdrawal
    {
        decimal Amount { get; set; }

        ApplicationUser CreatedBy { get; set; }
    }
}
