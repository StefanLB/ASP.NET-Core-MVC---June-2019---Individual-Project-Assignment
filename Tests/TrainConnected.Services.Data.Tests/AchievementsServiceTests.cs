namespace TrainConnected.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Data.Repositories;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.Achievements;
    using Xunit;

    public class AchievementsServiceTests
    {
        private readonly IRepository<Achievement> achievementsRepository;
        private readonly IRepository<Workout> workoutsRepository;
        private readonly IRepository<Booking> bookingsRepository;
        private readonly IRepository<WorkoutActivity> workoutActivitiesRepository;
        private readonly AchievementsService achievementsService;

        public AchievementsServiceTests()
        {
            var options = new DbContextOptionsBuilder<TrainConnectedDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new TrainConnectedDbContext(options);

            AutoMapperConfig.RegisterMappings(new[]
            {
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(WorkoutActivityEditInputModel).GetTypeInfo().Assembly,
            });

            this.achievementsRepository = new EfRepository<Achievement>(dbContext);
            this.workoutsRepository = new EfRepository<Workout>(dbContext);
            this.bookingsRepository = new EfRepository<Booking>(dbContext);
            this.workoutActivitiesRepository = new EfRepository<WorkoutActivity>(dbContext);

            this.achievementsService = new AchievementsService(this.achievementsRepository, this.workoutsRepository, this.bookingsRepository, this.workoutActivitiesRepository);
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllAchievements()
        {
            var userId = "testUserId";
            var achievementsDictionary = this.FillAllAchievements();

            foreach (var achievement in achievementsDictionary)
            {
                await this.achievementsRepository.AddAsync(new Achievement()
                {
                    Name = achievement.Key,
                    Description = achievement.Value,
                    TrainConnectedUserId = userId,
                });

            }

            await this.achievementsRepository.SaveChangesAsync();

            var expectedAchievementsCount = achievementsDictionary.Count;

            var actualAchievements = await this.achievementsService.GetAllAsync(userId);
            var actualAchievementsCount = actualAchievements.Count();

            Assert.Equal(expectedAchievementsCount, actualAchievementsCount);
        }

        [Fact]
        public async Task TestGetAllAsync_WithNoData_ShouldReturnNoAchievements()
        {
            var userId = "testUserId";

            IEnumerable<AchievementsAllViewModel> expectedResult = new List<AchievementsAllViewModel>();

            var actualResult = await this.achievementsService.GetAllAsync(userId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithCorrectData_ShouldReturnAchievement()
        {
            var userId = "testUserId";
            var achievementId = "testAchievementId";

            var testAchievement = new Achievement()
            {
                Id = achievementId,
                Name = ServiceConstants.Achievement.AdventurerAchievementName,
                Description = ServiceConstants.Achievement.AdventurerAchievementDescription,
                TrainConnectedUserId = userId,
            };

            await this.achievementsRepository.AddAsync(testAchievement);
            await this.achievementsRepository.SaveChangesAsync();

            var expectedAchievement = testAchievement;
            var actualAchievement = await this.achievementsService.GetDetailsAsync(achievementId, userId);

            Assert.Equal(expectedAchievement.Name, actualAchievement.Name);
            Assert.Equal(expectedAchievement.Description, actualAchievement.Description);
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectAchievementId_ShouldThrowNullRefEx()
        {
            var userId = "testUserId";
            var achievementId = "testAchievementId";
            var incorrectAchievementId = "incorrectTestAchievementId";

            var testAchievement = new Achievement()
            {
                Id = achievementId,
                Name = ServiceConstants.Achievement.AdventurerAchievementName,
                Description = ServiceConstants.Achievement.AdventurerAchievementDescription,
                TrainConnectedUserId = userId,
            };

            await this.achievementsRepository.AddAsync(testAchievement);
            await this.achievementsRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.achievementsService.GetDetailsAsync(incorrectAchievementId, userId));
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectUserId_ShouldThrowArgumentEx()
        {
            var userId = "testUserId";
            var incorrectUserId = "incorrectUserId";
            var achievementId = "testAchievementId";

            var testAchievement = new Achievement()
            {
                Id = achievementId,
                Name = ServiceConstants.Achievement.AdventurerAchievementName,
                Description = ServiceConstants.Achievement.AdventurerAchievementDescription,
                TrainConnectedUserId = userId,
            };

            await this.achievementsRepository.AddAsync(testAchievement);
            await this.achievementsRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<ArgumentException>(async () => await this.achievementsService.GetDetailsAsync(achievementId, incorrectUserId));
        }

        [Fact]
        public async Task TestCreateAchievementAsync_WithCorrectData_ShouldCreateAchievement()
        {
            var testAchievement = new Achievement
            {
                Name = "Testname",
                Description = "TestDecription",
                AchievedOn = DateTime.UtcNow,
                TrainConnectedUserId = "TestUserId",
            };

            await this.achievementsService.CreateAchievementAsync(
                testAchievement.Name,
                testAchievement.TrainConnectedUserId,
                testAchievement.Description,
                testAchievement.AchievedOn);

            var expectedAchievement = testAchievement;
            var actualAchievement = await this.achievementsRepository.All()
                .FirstOrDefaultAsync();

            Assert.Equal(expectedAchievement.Name, actualAchievement.Name);
            Assert.Equal(expectedAchievement.Description, actualAchievement.Description);
            Assert.Equal(expectedAchievement.AchievedOn, actualAchievement.AchievedOn);
            Assert.Equal(expectedAchievement.TrainConnectedUserId, actualAchievement.TrainConnectedUserId);
        }

        [Fact]
        public async Task TestCheckForAchievementsAsync_WithNoFullfilledConditions_ShouldCreateNoAchievements()
        {
            string testUserId = "TestUserId";

            await this.achievementsService.CheckForAchievementsAsync(testUserId);

            var actualResult = await this.achievementsRepository.All()
                .ToArrayAsync();

            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task TestCheckForAchievementsAsync_WithAllFullfilledConditions_ShouldCreateAllAchievements()
        {
            string testUserId = "TestUserId";

            await this.FulfillAllAchievementRequirements(this.bookingsRepository, this.workoutActivitiesRepository, this.workoutsRepository, testUserId);

            await this.achievementsService.CheckForAchievementsAsync(testUserId);

            var actualResult = await this.achievementsRepository.All()
                .ToArrayAsync();

            var actualResultAchievementNames = actualResult.Select(n => n.Name).ToList();

            var expectedAchievements = this.FillAllAchievements();

            foreach (var expectedAchievement in expectedAchievements)
            {
                Assert.Contains(expectedAchievement.Key, actualResultAchievementNames);
            }
        }

        private async Task FulfillAllAchievementRequirements(IRepository<Booking> bookingsRepository, IRepository<WorkoutActivity> workoutActivitiesRepository, IRepository<Workout> workoutsRepository, string testUserId)
        {
            for (int i = 0; i < 100; i++)
            {
                await this.workoutActivitiesRepository.AddAsync(new WorkoutActivity()
                {
                    Id = i.ToString(),
                    Name = i.ToString(),
                });
            }

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivitiesArray = await this.workoutActivitiesRepository.All().ToArrayAsync();

            for (int i = 0; i < 100; i++)
            {
                var userWorkout = new TrainConnectedUsersWorkouts()
                {
                    TrainConnectedUserId = testUserId,
                    WorkoutId = i.ToString(),
                };

                await this.workoutsRepository.AddAsync(new Workout()
                {
                    Id = i.ToString(),
                    Activity = workoutActivitiesArray[i],
                    ActivityId = i.ToString(),
                    Time = DateTime.Now.AddHours(-i),
                    MaxParticipants = 1,
                    Price = 100.00m,
                    Users = new HashSet<TrainConnectedUsersWorkouts>() { userWorkout },
                });
            }

            await this.workoutsRepository.SaveChangesAsync();

            for (int i = 0; i < 100; i++)
            {
                await this.bookingsRepository.AddAsync(new Booking()
                {
                    Id = i.ToString(),
                    Price = 100.00m,
                    WorkoutId = i.ToString(),
                    TrainConnectedUserId = testUserId,
                });
            }

            await this.bookingsRepository.SaveChangesAsync();
        }

        private Dictionary<string, string> FillAllAchievements()
        {
            var achievementsDictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Achievement.AdventurerAchievementName, ServiceConstants.Achievement.AdventurerAchievementDescription },
                { ServiceConstants.Achievement.BigSpenderAchievementName, ServiceConstants.Achievement.BigSpenderAchievementDescription },
                { ServiceConstants.Achievement.CantGetEnoughAchievementName, ServiceConstants.Achievement.CantGetEnoughAchievementName },
                { ServiceConstants.Achievement.CuttingItCloseAchievementName, ServiceConstants.Achievement.CuttingItCloseAchievementDescription },
                { ServiceConstants.Achievement.DoubleTroubleAchievementName, ServiceConstants.Achievement.DoubleTroubleAchievementDescription },
                { ServiceConstants.Achievement.EarlyBirdAchievementName, ServiceConstants.Achievement.EarlyBirdAchievementDescription },
                { ServiceConstants.Achievement.FirstResponderAchievementName, ServiceConstants.Achievement.FirstResponderAchievementDescription },
                { ServiceConstants.Achievement.FirstWorkoutAchievementName, ServiceConstants.Achievement.FirstWorkoutAchievementDescription },
                { ServiceConstants.Achievement.GettingStartedAchievementName, ServiceConstants.Achievement.GettingStartedAchievementDescription },
                { ServiceConstants.Achievement.MedicAchievementName, ServiceConstants.Achievement.MedicAchievementDescription },
                { ServiceConstants.Achievement.NightOwlAchievementName, ServiceConstants.Achievement.NightOwlAchievementDescription },
                { ServiceConstants.Achievement.VeteranAchievementName, ServiceConstants.Achievement.VeteranAchievementDescription },
            };

            return achievementsDictionary;
        }
    }
}
