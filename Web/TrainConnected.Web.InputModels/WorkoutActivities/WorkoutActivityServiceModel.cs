namespace TrainConnected.Web.InputModels.WorkoutActivities
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Services.Mapping;

    public class WorkoutActivityServiceModel : IMapFrom<WorkoutActivityCreateInputModel>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Icon { get; set; }
    }
}
