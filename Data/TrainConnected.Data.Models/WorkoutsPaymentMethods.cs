namespace TrainConnected.Data.Models
{
    public class WorkoutsPaymentMethods
    {
        public string WorkoutId { get; set; }
        public Workout Workout { get; set; }

        public string PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
