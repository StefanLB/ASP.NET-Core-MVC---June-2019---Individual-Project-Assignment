namespace TrainConnected.Web.ViewModels.WorkoutActivities
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutActivityDetailsViewModel : IMapFrom<WorkoutActivity>
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Activity Icon")]
        public string Icon { get; set; }
    }
}
