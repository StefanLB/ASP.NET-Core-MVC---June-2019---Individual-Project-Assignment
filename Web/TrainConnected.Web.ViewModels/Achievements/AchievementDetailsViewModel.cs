namespace TrainConnected.Web.ViewModels.Achievements
{
    using System;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class AchievementDetailsViewModel : IMapFrom<Achievement>
    {
        public string Name { get; set; }

        public DateTime AchievedOn { get; set; }

        public string Description { get; set; }
    }
}
