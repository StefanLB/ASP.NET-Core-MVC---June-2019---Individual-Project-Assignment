namespace TrainConnected.Data.Models.Contracts
{
    public interface IWorkoutsPaymentMethods
    {
        string WorkoutId { get; set; }
        Workout Workout { get; set; }

        string PaymentMethodId { get; set; }
        PaymentMethod PaymentMethod { get; set; }
    }
}
