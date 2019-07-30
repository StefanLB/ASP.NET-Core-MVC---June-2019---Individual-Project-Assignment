namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels.Workouts;

    public interface IWorkoutsService
    {
        Task<WorkoutDetailsViewModel> CreateAsync(WorkoutCreateInputModel workoutCreateInputModel, string userId);

        Task<WorkoutDetailsViewModel> GetDetailsAsync(string id, string userId);

        Task<WorkoutCancelViewModel> GetCancelDetailsAsync(string id, string userId);

        Task<IEnumerable<WorkoutsAllViewModel>> GetAllUpcomingAsync(string userId);

        Task<IEnumerable<WorkoutsHomeViewModel>> GetAllUpcomingHomeAsync();

        Task<IEnumerable<WorkoutsAllViewModel>> GetMyAsync(string userId);

        Task<IEnumerable<WorkoutsAllViewModel>> GetMyCreatedAsync(string userId);

        Task CancelAsync(string id, string userId);
    }
}
