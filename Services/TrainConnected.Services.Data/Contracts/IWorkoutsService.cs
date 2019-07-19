namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels.Workouts;

    public interface IWorkoutsService
    {
        Task<WorkoutDetailsViewModel> CreateAsync(WorkoutCreateInputModel workoutCreateInputModel, string userId);
        Task<WorkoutDetailsViewModel> GetDetailsAsync(string id);
        Task<IEnumerable<WorkoutsAllViewModel>> GetAllAsync();
    }
}
