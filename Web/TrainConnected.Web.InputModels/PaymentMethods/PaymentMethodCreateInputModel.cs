namespace TrainConnected.Web.InputModels.PaymentMethods
{
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class PaymentMethodCreateInputModel : IMapFrom<WorkoutActivity>
    {
        [Required]
        [StringLength(ModelConstants.PaymentMethod.NameMaxLength, MinimumLength = ModelConstants.PaymentMethod.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        [Display(Name = ModelConstants.PaymentMethod.PaymentInAdvanceNameDisplay)]
        public bool PaymentInAdvance { get; set; }
    }
}
