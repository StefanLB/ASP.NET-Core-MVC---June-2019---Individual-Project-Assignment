namespace TrainConnected.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class PaymentMethod : BaseDeletableModel<string>, IPaymentMethod
    {
        [Required]
        [StringLength(ModelConstants.PaymentMethod.NameMaxLength, MinimumLength = ModelConstants.PaymentMethod.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        public bool PaymentInAdvance { get; set; }
    }
}
