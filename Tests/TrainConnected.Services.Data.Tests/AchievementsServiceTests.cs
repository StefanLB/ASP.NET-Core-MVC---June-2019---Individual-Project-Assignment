namespace TrainConnected.Services.Data.Tests
{
    using System;
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
            var result = Achievement
        }
    }
}
