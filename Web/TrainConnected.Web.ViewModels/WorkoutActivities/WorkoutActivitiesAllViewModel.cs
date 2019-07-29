namespace TrainConnected.Web.ViewModels.WorkoutActivities
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutActivitiesAllViewModel : IMapFrom<WorkoutActivity>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }
    }
}
