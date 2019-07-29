namespace TrainConnected.Web.InputModels.WorkoutActivities
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutActivityCreateInputModel : IMapFrom<WorkoutActivity>
    {
        [Required]
        [StringLength(ModelConstants.WorkoutActivity.NameMaxLength, MinimumLength = ModelConstants.WorkoutActivity.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        [StringLength(ModelConstants.WorkoutActivity.DescriptionMaxLength, MinimumLength = ModelConstants.WorkoutActivity.DescriptionMinLength, ErrorMessage = ModelConstants.DescriptionLengthError)]
        public string Description { get; set; }

        [Required]
        public IFormFile Icon { get; set; }
    }
}
