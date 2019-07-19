namespace TrainConnected.Services.Data.Contracts
{
    using System.Threading.Tasks;
    using TrainConnected.Web.InputModels.Bookings;
    using TrainConnected.Web.ViewModels.Bookings;
    using TrainConnected.Web.ViewModels.Workouts;

    public interface IBookingsService
    {
        Task<WorkoutDetailsViewModel> GetWorkoutByIdAsync(string id);
        Task<BookingDetailsViewModel> CreateAsync(BookingCreateInputModel bookingCreateInputModel, string userId);
        Task<BookingDetailsViewModel> GetDetailsAsync(string id);
    }
}
