namespace TrainConnected.Web.InputModels.WorkoutActivities
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Services.Mapping;

    public class WorkoutActivityServiceModel : IMapFrom<WorkoutActivityCreateInputModel>
    {
        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Description { get; set; }

        [Required]
        public string ActivityIcon { get; set; }
    }
}
