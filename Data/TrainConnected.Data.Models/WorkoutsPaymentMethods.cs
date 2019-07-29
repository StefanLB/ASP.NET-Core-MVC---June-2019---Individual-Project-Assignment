namespace TrainConnected.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class WorkoutsPaymentMethods
    {
        [Required]
        public string WorkoutId { get; set; }

        public Workout Workout { get; set; }

        [Required]
        public string PaymentMethodId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}
