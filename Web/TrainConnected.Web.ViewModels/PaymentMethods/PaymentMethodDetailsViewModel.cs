namespace TrainConnected.Web.ViewModels.PaymentMethods
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class PaymentMethodDetailsViewModel : IMapFrom<PaymentMethod>
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Name { get; set; }

        [Required]
        public bool PaymentInAdvance { get; set; }
    }
}
