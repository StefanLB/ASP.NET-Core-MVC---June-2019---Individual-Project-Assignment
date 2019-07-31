namespace TrainConnected.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.ViewModels.Achievements;

    public class AchievementsService : IAchievementsService
    {
        private readonly IRepository<Achievement> achievementsRepository;
        private readonly IRepository<Workout> workoutsRepository;
        private readonly IRepository<Booking> bookingsRepository;
        private readonly IRepository<WorkoutActivity> workoutActivitiesRepository;

        public AchievementsService(IRepository<Achievement> achievementsRepository, IRepository<Workout> workoutsRepository, IRepository<Booking> bookingsRepository, IRepository<WorkoutActivity> workoutActivitiesRepository)
        {
            this.achievementsRepository = achievementsRepository;
            this.workoutsRepository = workoutsRepository;
            this.bookingsRepository = bookingsRepository;
            this.workoutActivitiesRepository = workoutActivitiesRepository;
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

        public async Task<AchievementDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var achievement = await this.achievementsRepository.All()
                .Where(x => x.Id == id)
                .To<AchievementDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (achievement == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Achievement.NullReferenceAchievementId, id));
            }

            var achievedByUserId = await this.achievementsRepository.All()
                .Where(x => x.Id == id)
                .Select(x => x.TrainConnectedUserId)
                .FirstOrDefaultAsync();

            if (achievedByUserId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Achievement.ArgumentUserIdMismatch, userId));
            }

            return achievement;
        }

        // TODO: Use Reflection
        public async Task CheckForAchievementsAsync(string userId)
        {
            var achievements = await this.achievementsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .ToArrayAsync();

            var userWorkouts = await this.workoutsRepository.All()
                .Where(x => x.Users.Any(y => y.TrainConnectedUserId == userId))
                .Where(x => x.Time < DateTime.UtcNow.ToLocalTime())
                .ToArrayAsync();

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.FirstWorkoutAchievementName))
            {
                if (userWorkouts.Any())
                {
                    var achievementName = ServiceConstants.Achievement.FirstWorkoutAchievementName;
                    var description = ServiceConstants.Achievement.FirstWorkoutAchievementDescription;
                    var achievedOn = userWorkouts.OrderBy(x => x.Time).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.GettingStartedAchievementName))
            {
                if (userWorkouts.Count() >= 10)
                {
                    var achievementName = ServiceConstants.Achievement.GettingStartedAchievementName;
                    var description = ServiceConstants.Achievement.GettingStartedAchievementDescription;
                    var achievedOn = userWorkouts.OrderBy(x => x.Time).Skip(9).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.VeteranAchievementName))
            {
                if (userWorkouts.Count() >= 100)
                {
                    var achievementName = ServiceConstants.Achievement.VeteranAchievementName;
                    var description = ServiceConstants.Achievement.VeteranAchievementDescription;
                    var achievedOn = userWorkouts.OrderBy(x => x.Time).Skip(99).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.FirstResponderAchievementName))
            {
                var firstToSignUpCounter = 0;

                var bookings = await this.bookingsRepository.All()
                    .Where(x => userWorkouts.Select(y => y.Id).Contains(x.WorkoutId))
                    .ToArrayAsync();

                foreach (var workout in userWorkouts.OrderBy(x => x.Time))
                {
                    workout.Bookings = bookings.Where(x => x.WorkoutId == workout.Id).ToList();

                    if (workout.Bookings.OrderBy(x => x.CreatedOn).First().TrainConnectedUserId == userId)
                    {
                        firstToSignUpCounter++;

                        if (firstToSignUpCounter >= 10)
                        {
                            var achievementName = ServiceConstants.Achievement.FirstResponderAchievementName;
                            var description = ServiceConstants.Achievement.FirstResponderAchievementDescription;
                            var achievedOn = workout.Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.CuttingItCloseAchievementName))
            {
                var lastToSignUpCounter = 0;

                var bookings = await this.bookingsRepository.All()
                    .Where(x => userWorkouts.Select(y => y.Id).Contains(x.WorkoutId))
                    .ToArrayAsync();

                foreach (var workout in userWorkouts.OrderBy(x => x.Time))
                {
                    if (workout.Bookings.Count == workout.MaxParticipants)
                    {
                        workout.Bookings = bookings.Where(x => x.WorkoutId == workout.Id).ToList();

                        if (workout.Bookings.OrderByDescending(x => x.CreatedOn).First().TrainConnectedUserId == userId)
                        {
                            lastToSignUpCounter++;

                            if (lastToSignUpCounter >= 10)
                            {
                                var achievementName = ServiceConstants.Achievement.CuttingItCloseAchievementName;
                                var description = ServiceConstants.Achievement.CuttingItCloseAchievementDescription;
                                var achievedOn = workout.Time;

                                await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                                break;
                            }
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.BigSpenderAchievementName))
            {
                if (userWorkouts.Sum(x => x.Price) >= 1000)
                {
                    var achievementName = ServiceConstants.Achievement.BigSpenderAchievementName;
                    var description = ServiceConstants.Achievement.BigSpenderAchievementDescription;
                    var achievedOn = DateTime.MinValue;

                    decimal totalSpent = 0.00m;

                    foreach (var workout in userWorkouts.OrderBy(x => x.Time))
                    {
                        totalSpent += workout.Price;

                        if (totalSpent >= 1000)
                        {
                            achievedOn = workout.Time;

                            break;
                        }
                    }

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.AdventurerAchievementName))
            {
                var differentActivitiesCounter = 0;
                var differentActivitiesList = new List<string>();

                var activities = await this.workoutActivitiesRepository.All()
                    .Where(x => userWorkouts.Select(y => y.ActivityId).Contains(x.Id))
                    .ToArrayAsync();

                if (activities.Distinct().Count() >= 10)
                {
                    foreach (var workout in userWorkouts.OrderBy(x => x.Time))
                    {
                        workout.Activity = activities.Where(x => x.Id == workout.ActivityId).FirstOrDefault();

                        if (!differentActivitiesList.Contains(workout.Activity.Name))
                        {
                            differentActivitiesCounter++;
                            differentActivitiesList.Add(workout.Activity.Name);

                            if (differentActivitiesCounter >= 10)
                            {
                                var achievementName = ServiceConstants.Achievement.AdventurerAchievementName;
                                var description = ServiceConstants.Achievement.AdventurerAchievementDescription;
                                var achievedOn = workout.Time;

                                await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                                break;
                            }
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.DoubleTroubleAchievementName))
            {
                if (userWorkouts.Count() > 1)
                {
                    userWorkouts = userWorkouts.OrderBy(x => x.Time).ToArray();
                    var sameDayCounter = 1;

                    for (int i = 1; i < userWorkouts.Count(); i++)
                    {
                        if (userWorkouts[i].Time.Date == userWorkouts[i - 1].Time.Date)
                        {
                            sameDayCounter++;
                        }

                        if (sameDayCounter >= 2)
                        {
                            var achievementName = ServiceConstants.Achievement.DoubleTroubleAchievementName;
                            var description = ServiceConstants.Achievement.DoubleTroubleAchievementDescription;
                            var achievedOn = userWorkouts[i].Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.CantGetEnoughAchievementName))
            {
                if (userWorkouts.Count() > 2)
                {
                    userWorkouts = userWorkouts.OrderBy(x => x.Time).ToArray();
                    var sameDayCounter = 1;

                    for (int i = 1; i < userWorkouts.Count(); i++)
                    {
                        if (userWorkouts[i].Time.Date == userWorkouts[i - 1].Time.Date)
                        {
                            sameDayCounter++;
                        }

                        if (sameDayCounter >= 3)
                        {
                            var achievementName = ServiceConstants.Achievement.CantGetEnoughAchievementName;
                            var description = ServiceConstants.Achievement.CantGetEnoughAchievementDescription;
                            var achievedOn = userWorkouts[i].Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.MedicAchievementName))
            {
                if (userWorkouts.Count() > 3)
                {
                    userWorkouts = userWorkouts.OrderBy(x => x.Time).ToArray();
                    var sameDayCounter = 1;

                    for (int i = 1; i < userWorkouts.Count(); i++)
                    {
                        if (userWorkouts[i].Time.Date == userWorkouts[i - 1].Time.Date)
                        {
                            sameDayCounter++;
                        }

                        if (sameDayCounter >= 4)
                        {
                            var achievementName = ServiceConstants.Achievement.MedicAchievementName;
                            var description = ServiceConstants.Achievement.MedicAchievementDescription;
                            var achievedOn = userWorkouts[i].Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.EarlyBirdAchievementName))
            {
                if (userWorkouts.Any(x => x.Time.Hour < 8))
                {
                    var achievementName = ServiceConstants.Achievement.EarlyBirdAchievementName;
                    var description = ServiceConstants.Achievement.EarlyBirdAchievementDescription;
                    var achievedOn = userWorkouts.Where(x => x.Time.Hour < 8).OrderBy(x => x.Time).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == ServiceConstants.Achievement.NightOwlAchievementName))
            {
                if (userWorkouts.Any(x => x.Time.Hour >= 20))
                {
                    var achievementName = ServiceConstants.Achievement.NightOwlAchievementName;
                    var description = ServiceConstants.Achievement.NightOwlAchievementDescription;
                    var achievedOn = userWorkouts.Where(x => x.Time.Hour >= 20).OrderBy(x => x.Time).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }
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
