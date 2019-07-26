namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.InputModels.Bookings;
    using TrainConnected.Web.ViewModels.Bookings;
    using TrainConnected.Web.ViewModels.Workouts;

    public interface IBookingsService
    {
        Task<BookingDetailsViewModel> CreateAsync(BookingCreateInputModel bookingCreateInputModel, string userId);

        Task<BookingDetailsViewModel> GetDetailsAsync(string id);

        Task<IEnumerable<BookingsAllViewModel>> GetAllAsync(string userId);

        Task<IEnumerable<BookingsAllViewModel>> GetAllHistoryAsync(string userId);

        Task CancelAsync(string id);
    }
}
