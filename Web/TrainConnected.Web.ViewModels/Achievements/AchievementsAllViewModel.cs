namespace TrainConnected.Web.ViewModels.Achievements
{
    using System;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class AchievementsAllViewModel : IMapFrom<Achievement>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime AchievedOn { get; set; }

        public string Description { get; set; }
    }
}
