namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class PaymentMethod : BaseDeletableModel<string>, IPaymentMethod
    {
        public string Name { get; set; }

        public bool PaymentInAdvance { get; set; }
    }
}
