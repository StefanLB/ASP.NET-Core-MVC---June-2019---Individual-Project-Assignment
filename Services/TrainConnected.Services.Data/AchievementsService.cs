namespace TrainConnected.Services.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrainConnected.Data;
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
        private readonly IRepository<Booking> bookingsRepository;
        private readonly IRepository<WorkoutActivity> workoutActivitiesRepository;

        public AchievementsService(IRepository<Achievement> achievementsRepository, IRepository<TrainConnectedUser> usersRepository, IRepository<Workout> workoutsRepository, IRepository<Booking> bookingsRepository, IRepository<WorkoutActivity> workoutActivitiesRepository)
        {
            this.achievementsRepository = achievementsRepository;
            this.usersRepository = usersRepository;
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
            // TODO: add icons for each achievement
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

            if (!achievements.Any(x => x.Name == "Veteran"))
            {
                if (userWorkouts.Count() >= 100)
                {
                    var achievementName = "Veteran";
                    var description = "Attend 100 workouts, regardless of activity type!";
                    var achievedOn = userWorkouts.OrderBy(x => x.Time).Skip(99).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == "First Responder"))
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
                            var achievementName = "First Responder";
                            var description = "Be the first person to sign up for 10 different workouts!";
                            var achievedOn = workout.Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == "Cutting it Close"))
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
                                var achievementName = "Cutting it Close";
                                var description = "Sign up for the last spot for 10 different workouts!";
                                var achievedOn = workout.Time;

                                await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                                break;
                            }
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == "Big Spender"))
            {
                if (userWorkouts.Sum(x => x.Price) >= 1000)
                {
                    var achievementName = "Big Spender";
                    var description = "Spend over BGN 1000.00 on workouts!";
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

            if (!achievements.Any(x => x.Name == "Adventurer"))
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
                                var achievementName = "Adventurer";
                                var description = "Sign up for 10 different types of workouts!";
                                var achievedOn = workout.Time;

                                await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                                break;
                            }
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == "Double Trouble"))
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
                            var achievementName = "Double Trouble";
                            var description = "Attend two workouts in one day!";
                            var achievedOn = userWorkouts[i].Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == "Can't Get Enough"))
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
                            var achievementName = "Can't Get Enough";
                            var description = "Attend three workouts in one day!";
                            var achievedOn = userWorkouts[i].Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == "MEDIC!!!"))
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
                            var achievementName = "MEDIC!!!";
                            var description = "Attend four workouts in one day!";
                            var achievedOn = userWorkouts[i].Time;

                            await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);

                            break;
                        }
                    }
                }
            }

            if (!achievements.Any(x => x.Name == "Early Bird"))
            {
                if (userWorkouts.Any(x => x.Time.Hour < 8))
                {
                    var achievementName = "Early Bird";
                    var description = "Attend a workout before 08:00 a.m.!";
                    var achievedOn = userWorkouts.Where(x => x.Time.Hour < 8).OrderBy(x => x.Time).First().Time;

                    await this.CreateAchievementAsync(achievementName, userId, description, achievedOn);
                }
            }

            if (!achievements.Any(x => x.Name == "Night Owl"))
            {
                if (userWorkouts.Any(x => x.Time.Hour >= 20))
                {
                    var achievementName = "Night Owl";
                    var description = "Attend a workout after 08:00 p.m.!";
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
