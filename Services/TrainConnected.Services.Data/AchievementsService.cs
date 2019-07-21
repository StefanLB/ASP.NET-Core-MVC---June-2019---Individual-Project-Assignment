namespace TrainConnected.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.ViewModels.Achievements;

    public class AchievementsService : IAchievementsService
    {
        private readonly IRepository<Achievement> achievementsRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<Workout> workoutsRepository;

        public AchievementsService(IRepository<Achievement> achievementsRepository, IRepository<TrainConnectedUser> usersRepository, IRepository<Workout> workoutsRepository)
        {
            this.achievementsRepository = achievementsRepository;
            this.usersRepository = usersRepository;
            this.workoutsRepository = workoutsRepository;
        }

        public async Task<IEnumerable<AchievementsAllViewModel>> GetAllAsync(string userId)
        {
            var achievements = await this.achievementsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .To<AchievementsAllViewModel>()
                .OrderBy(x => x.AchievedOn)
                .ToArrayAsync();

            return achievements;
        }

        public async Task<AchievementDetailsViewModel> GetDetailsAsync(string id)
        {
            var achievement = await this.achievementsRepository.All()
                .Where(x => x.Id == id)
                .To<AchievementDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (achievement == null)
            {
                throw new InvalidOperationException();
            }

            return achievement;
        }

        public async Task CheckForAchievementsAsync(string userId)
        {
            var achievements = await this.achievementsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .ToArrayAsync();

            var userWorkouts = await this.workoutsRepository.All()
                .Where(x => x.Users.Any(y => y.TrainConnectedUserId == userId))
                .Where(x => x.Time < DateTime.UtcNow)
                .ToArrayAsync();

            // TODO: extract constants, use Reflection !!!
            if (!achievements.Any(x => x.Name == "First Workout"))
            {
                if (userWorkouts.Any())
                {
                    var achievementName = "First Workout";
                    var description = "Your first ever workout in TrainConnected!";
                    var achievedOn = userWorkouts.OrderBy(x => x.Time).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == "Getting Started"))
            {
                if (userWorkouts.Count() >= 10)
                {
                    var achievementName = "Getting Started";
                    var description = "Attend 10 workouts, regardless of activity type!";
                    var achievedOn = userWorkouts.OrderBy(x => x.Time).Skip(9).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == "Early Bird"))
            {
                var firstToSignUpCounter = 0;

                foreach (var workout in userWorkouts.OrderBy(x => x.Time))
                {
                    if (workout.Bookings.OrderBy(x => x.CreatedOn).First().TrainConnectedUserId == userId)
                    {
                        firstToSignUpCounter++;

                        if (firstToSignUpCounter >= 10)
                        {
                            var achievementName = "Early Bird";
                            var description = "Be the first person to sign up for 10 different workouts!";
                            var achievedOn = workout.Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            //if (!achievements.Any(x => x.Name == "Getting Started"))
            //{
            //    if (true)
            //    {
            //        var achievementName = "";
            //        var description = "";

            //        await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
            //    }
            //}

            //if (!achievements.Any(x => x.Name == "Getting Started"))
            //{
            //    if (true)
            //    {
            //        var achievementName = "";
            //        var description = "";

            //        await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
            //    }
            //}

            //if (!achievements.Any(x => x.Name == "Getting Started"))
            //{
            //    if (true)
            //    {
            //        var achievementName = "";
            //        var description = "";

            //        await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
            //    }
            //}

            //if (!achievements.Any(x => x.Name == "Getting Started"))
            //{
            //    if (true)
            //    {
            //        var achievementName = "";
            //        var description = "";

            //        await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
            //    }
            //}

            //if (!achievements.Any(x => x.Name == "Getting Started"))
            //{
            //    if (true)
            //    {
            //        var achievementName = "";
            //        var description = "";

            //        await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
            //    }
            //}

            //if (!achievements.Any(x => x.Name == "Getting Started"))
            //{
            //    if (true)
            //    {
            //        var achievementName = "";
            //        var description = "";

            //        await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
            //    }
            //}

            //if (!achievements.Any(x => x.Name == "Getting Started"))
            //{
            //    if (true)
            //    {
            //        var achievementName = "";
            //        var description = "";

            //        await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
            //    }
            //}

        }

        public async Task CreateAchievementAsync(string achievementName, string userId, string description, DateTime achievedOn)
        {
            await this.achievementsRepository.AddAsync(new Achievement
            {
                Name = achievementName,
                Description = description,
                AchievedOn = achievedOn,
                TrainConnectedUserId = userId,
            });

            await this.achievementsRepository.SaveChangesAsync();
        }
    }
}
