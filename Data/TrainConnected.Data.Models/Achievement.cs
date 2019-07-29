namespace TrainConnected.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class Achievement : BaseDeletableModel<string>, IAchievement
    {
        [Required]
        [StringLength(ModelConstants.Achievement.NameMaxLength, MinimumLength = ModelConstants.Achievement.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        public DateTime AchievedOn { get; set; }

        [Required]
        [StringLength(ModelConstants.Achievement.DescriptionMaxLength, MinimumLength = ModelConstants.Achievement.DescriptionMinLength, ErrorMessage = ModelConstants.DescriptionLengthError)]
        public string Description { get; set; }

        [Required]
        public string TrainConnectedUserId { get; set; }

        public TrainConnectedUser TrainConnectedUser { get; set; }

    }
}
