namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Common.Models;

    public class PaymentMethod : BaseDeletableModel<string>, IPaymentMethod
    {
        public string Name { get; set; }

        public bool PaymentInAdvance { get; set; }
    }
}
