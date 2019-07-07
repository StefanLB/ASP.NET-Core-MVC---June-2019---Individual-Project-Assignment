namespace TrainConnected.Data.Models
{
    using System;
    using TrainConnected.Data.Common.Models;

    public class Achievement : BaseDeletableModel<string>, IAchievement
    {
        public string Name { get; set; }

        public DateTime? FirstAchievedOn { get; set; }

        public DateTime? LastAchievedOn { get; set; }

        public int TimesAchieved { get; set; }

        public string Description { get; set; }
    }
}
