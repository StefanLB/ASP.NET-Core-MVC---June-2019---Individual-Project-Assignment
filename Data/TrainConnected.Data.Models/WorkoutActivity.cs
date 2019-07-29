namespace TrainConnected.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class WorkoutActivity : BaseDeletableModel<string>, IWorkoutActivity
    {
        [Required]
        [StringLength(ModelConstants.WorkoutActivity.NameMaxLength, MinimumLength = ModelConstants.WorkoutActivity.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        [StringLength(ModelConstants.WorkoutActivity.DescriptionMaxLength, MinimumLength = ModelConstants.WorkoutActivity.DescriptionMinLength, ErrorMessage = ModelConstants.DescriptionLengthError)]
        public string Description { get; set; }

        [Required]
        public string Icon { get; set; }
    }
}
