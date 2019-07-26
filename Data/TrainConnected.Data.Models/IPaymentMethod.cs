namespace TrainConnected.Data.Models
{
    public interface IPaymentMethod
    {
        string Name { get; set; }

        bool PaymentInAdvance { get; set; }
    }
}
