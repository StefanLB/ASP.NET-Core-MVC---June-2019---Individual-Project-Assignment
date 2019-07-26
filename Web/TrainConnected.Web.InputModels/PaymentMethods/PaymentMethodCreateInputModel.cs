namespace TrainConnected.Web.InputModels.PaymentMethods
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class PaymentMethodCreateInputModel : IMapFrom<WorkoutActivity>
    {
        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Name { get; set; }

        [Required]
        [Display(Name ="Payment In Advance")]
        public bool PaymentInAdvance { get; set; }
    }
}
