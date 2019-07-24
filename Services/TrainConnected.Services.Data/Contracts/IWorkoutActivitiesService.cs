namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels.WorkoutActivities;

    public interface IWorkoutActivitiesService
    {
        Task<WorkoutActivityDetailsViewModel> GetDetailsAsync(string id);

        Task<WorkoutActivityEditInputModel> GetEditDetailsAsync(string id);

        Task<IEnumerable<WorkoutActivitiesAllViewModel>> GetAllAsync();

        Task<WorkoutActivityDetailsViewModel> CreateAsync(WorkoutActivityServiceModel workoutActivityServiceModel);

        Task UpdateAsync(WorkoutActivityEditInputModel workoutActivityEditInputModel);

        Task DeleteAsync(string id);
    }
}
