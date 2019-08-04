namespace TrainConnected.Web.ViewModels.Achievements
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class AchievementDetailsViewModel : IMapFrom<Achievement>
    {
        public string Name { get; set; }

        [Display(Name = ModelConstants.Achievement.AchievedOnNameDisplay)]
        public DateTime AchievedOn { get; set; }

        public string Description { get; set; }
    }
}
