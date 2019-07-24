namespace TrainConnected.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.ViewModels.Achievements;

    public interface IAchievementsService
    {
        Task<IEnumerable<AchievementsAllViewModel>> GetAllAsync(string userId);

        Task<AchievementDetailsViewModel> GetDetailsAsync(string id);

        Task CheckForAchievementsAsync(string userId);

        Task CreateAchievementAsync(string achievementName, string userId, string description, DateTime achievedOn);
    }
}
