namespace TrainConnected.Data.Models
{
    public interface IWorkoutsPaymentMethods
    {
        string WorkoutId { get; set; }
        Workout Workout { get; set; }

        string PaymentMethodId { get; set; }
        PaymentMethod PaymentMethod { get; set; }
    }
}
