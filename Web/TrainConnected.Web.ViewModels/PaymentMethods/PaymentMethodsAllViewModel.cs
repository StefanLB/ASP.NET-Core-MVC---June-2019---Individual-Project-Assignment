namespace TrainConnected.Web.ViewModels.PaymentMethods
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class PaymentMethodsAllViewModel : IMapFrom<PaymentMethod>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [Display(Name = ModelConstants.PaymentMethod.PaymentInAdvanceNameDisplay)]
        public bool PaymentInAdvance { get; set; }
    }
}
