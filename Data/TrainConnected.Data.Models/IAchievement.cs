namespace TrainConnected.Data.Models
{
    using System;

    public interface IAchievement
    {
        string Name { get; set; }

        DateTime? FirstAchievedOn { get; set; }

        DateTime? LastAchievedOn { get; set; }

        int TimesAchieved { get; set; }

        string Description { get; set; }
    }
}
