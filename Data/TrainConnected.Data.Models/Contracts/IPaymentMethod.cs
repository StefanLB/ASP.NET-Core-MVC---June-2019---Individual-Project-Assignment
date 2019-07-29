namespace TrainConnected.Data.Models.Contracts
{
    public interface IPaymentMethod
    {
        string Name { get; set; }

        bool PaymentInAdvance { get; set; }
    }
}
