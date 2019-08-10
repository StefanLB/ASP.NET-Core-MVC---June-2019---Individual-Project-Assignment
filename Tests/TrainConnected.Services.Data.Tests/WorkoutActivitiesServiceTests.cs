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
    using TrainConnected.Web.ViewModels.WorkoutActivities;
    using Xunit;

    public class WorkoutActivitiesServiceTests
    {
        private readonly IRepository<WorkoutActivity> workoutActivitiesRepository;
        private readonly WorkoutActivitiesService workoutActivitiesService;

        public WorkoutActivitiesServiceTests()
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

            this.workoutActivitiesRepository = new EfRepository<WorkoutActivity>(dbContext);
            this.workoutActivitiesService = new WorkoutActivitiesService(this.workoutActivitiesRepository);
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllWorkoutActivities()
        {
            var workoutActivities = new List<WorkoutActivity>()
            {
                new WorkoutActivity
                {
                    Name = "Jogging",
                    Description = "JoggingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Name = "Boxing",
                    Description = "BoxingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Name = "Rowing",
                    Description = "RowingDescription",
                    Icon = "ActivityIconUrl",
                },
            };

            foreach (var wa in workoutActivities)
            {
                await this.workoutActivitiesRepository.AddAsync(wa);
            }

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var expectedResult = await this.workoutActivitiesRepository.All()
                .To<WorkoutActivitiesAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.workoutActivitiesService.GetAllAsync();

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(wa =>
                    wa.Name == result.Name
                    && wa.Description == result.Description),
                    "WorkoutActivitiesService GetAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllAsync_WithNoData_ShouldReturnEmptyArray()
        {
            var result = await this.workoutActivitiesService.GetAllAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithTestData_ShouldReturnWorkoutActivity()
        {
            var testActivityId = "testActivityId";

            var workoutActivities = new List<WorkoutActivity>()
            {
                new WorkoutActivity
                {
                    Id = testActivityId,
                    Name = "Jogging",
                    Description = "JoggingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Name = "Boxing",
                    Description = "BoxingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Name = "Rowing",
                    Description = "RowingDescription",
                    Icon = "ActivityIconUrl",
                },
            };

            foreach (var wa in workoutActivities)
            {
                await this.workoutActivitiesRepository.AddAsync(wa);
            }

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var expectedResult = await this.workoutActivitiesRepository.All()
                .Where(wa => wa.Id == testActivityId)
                .To<WorkoutActivityDetailsViewModel>()
                .FirstOrDefaultAsync();

            var actualResult = await this.workoutActivitiesService.GetDetailsAsync(testActivityId);

            Assert.Equal(expectedResult.Name, actualResult.Name);
            Assert.Equal(expectedResult.Icon, actualResult.Icon);
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectId_ShouldThrowNullRefEx()
        {
            var incorrectActivityId = "incorrectActivityId";

            var workoutActivities = new List<WorkoutActivity>()
            {
                new WorkoutActivity
                {
                    Id = "testActivityId1",
                    Name = "Jogging",
                    Description = "JoggingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Id = "testActivityId2",
                    Name = "Boxing",
                    Description = "BoxingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Id = "testActivityId3",
                    Name = "Rowing",
                    Description = "RowingDescription",
                    Icon = "ActivityIconUrl",
                },
            };

            foreach (var wa in workoutActivities)
            {
                await this.workoutActivitiesRepository.AddAsync(wa);
            }

            await this.workoutActivitiesRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutActivitiesService.GetDetailsAsync(incorrectActivityId));
        }

        [Fact]
        public async Task TestCreateAsync_WithTestData_ShouldCreateWorkoutActivity()
        {
            var testActivityName = "testActivityName";
            var testActivityDescription = "testActivityDescription";
            var testIcon = "testIconUrl";

            var workoutActivityServiceModel = new WorkoutActivityServiceModel()
            {
                Name = testActivityName,
                Description = testActivityDescription,
                Icon = testIcon,
            };

            var actualResult = await this.workoutActivitiesService.CreateAsync(workoutActivityServiceModel);

            var expectedResult = await this.workoutActivitiesRepository.All()
                .Where(n => n.Name == testActivityName)
                .To<WorkoutActivityDetailsViewModel>()
                .FirstOrDefaultAsync();

            Assert.Equal(actualResult.Name, expectedResult.Name);
            Assert.Equal(actualResult.Icon, expectedResult.Icon);
            Assert.Equal(actualResult.Id, expectedResult.Id);
        }

        [Fact]
        public async Task TestCreateAsync_WithExistingName_ShouldReturnAlreadyExistingActivity()
        {
            var testActivityName = "testActivityName";
            var testActivityDescription = "testActivityDescription";
            var testIcon = "testIconUrl";

            await this.workoutActivitiesRepository.AddAsync(new WorkoutActivity()
            {
                Name = testActivityName,
            });

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivityServiceModel = new WorkoutActivityServiceModel()
            {
                Name = testActivityName,
                Description = testActivityDescription,
                Icon = testIcon,
            };

            var actualResult = await this.workoutActivitiesService.CreateAsync(workoutActivityServiceModel);

            var expectedResult = await this.workoutActivitiesRepository.All()
                .Where(n => n.Name == testActivityName)
                .To<WorkoutActivityDetailsViewModel>()
                .FirstOrDefaultAsync();

            Assert.Equal(actualResult.Name, expectedResult.Name);
            Assert.Equal(actualResult.Icon, expectedResult.Icon);
            Assert.Equal(actualResult.Id, expectedResult.Id);
        }

        [Fact]
        public async Task TestGetEditDetailsAsync_WithTestData_ShouldReturnWorkoutActivity()
        {
            var testActivityId = "testActivityId";

            var workoutActivities = new List<WorkoutActivity>()
            {
                new WorkoutActivity
                {
                    Id = testActivityId,
                    Name = "Jogging",
                    Description = "JoggingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Name = "Boxing",
                    Description = "BoxingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Name = "Rowing",
                    Description = "RowingDescription",
                    Icon = "ActivityIconUrl",
                },
            };

            foreach (var wa in workoutActivities)
            {
                await this.workoutActivitiesRepository.AddAsync(wa);
            }

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var expectedResult = await this.workoutActivitiesRepository.All()
                .Where(wa => wa.Id == testActivityId)
                .To<WorkoutActivityEditInputModel>()
                .FirstOrDefaultAsync();

            var actualResult = await this.workoutActivitiesService.GetEditDetailsAsync(testActivityId);

            Assert.Equal(expectedResult.Name, actualResult.Name);
            Assert.Equal(expectedResult.Id, actualResult.Id);
        }

        [Fact]
        public async Task TestGetEditDetailsAsync_WithIncorrectId_ShouldThrowNullRefEx()
        {
            var incorrectActivityId = "incorrectActivityId";

            var workoutActivities = new List<WorkoutActivity>()
            {
                new WorkoutActivity
                {
                    Id = "testActivityId1",
                    Name = "Jogging",
                    Description = "JoggingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Id = "testActivityId2",
                    Name = "Boxing",
                    Description = "BoxingDescription",
                    Icon = "ActivityIconUrl",
                },
                new WorkoutActivity
                {
                    Id = "testActivityId3",
                    Name = "Rowing",
                    Description = "RowingDescription",
                    Icon = "ActivityIconUrl",
                },
            };

            foreach (var wa in workoutActivities)
            {
                await this.workoutActivitiesRepository.AddAsync(wa);
            }

            await this.workoutActivitiesRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutActivitiesService.GetEditDetailsAsync(incorrectActivityId));
        }

        [Fact]
        public async Task TestUpdateAsync_WithTestData_ShouldUpdateWorkoutActivity()
        {
            var testActivityId = "testActivityId";
            var testInitialName = "testInitialName";
            var testUpdatedName = "testUpdatedName";
            var testInitialDescription = "testInitialDescription";
            var testUpdatedDescription = "testUpdatedDescription";

            var workoutActivity = new WorkoutActivity
            {
                Id = testActivityId,
                Name = testInitialName,
                Description = testInitialDescription,
                Icon = "ActivityIconUrl",
            };

            await this.workoutActivitiesRepository.AddAsync(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivityEditInputModel = new WorkoutActivityEditInputModel()
            {
                Id = testActivityId,
                Name = testUpdatedName,
                Description = testUpdatedDescription,
            };

            await this.workoutActivitiesService.UpdateAsync(workoutActivityEditInputModel);

            var result = await this.workoutActivitiesRepository.All()
                .Where(wa => wa.Id == testActivityId)
                .FirstOrDefaultAsync();

            Assert.Equal(testUpdatedName, result.Name);
            Assert.Equal(testUpdatedDescription, result.Description);
        }

        [Fact]
        public async Task TestUpdateAsync_WithIncorrectId_ShouldThrowNullRefEx()
        {
            var testActivityId = "testActivityId";
            var incorrectActivityId = "incorrectActivityId";
            var testInitialName = "testInitialName";
            var testUpdatedName = "testUpdatedName";
            var testInitialDescription = "testInitialDescription";
            var testUpdatedDescription = "testUpdatedDescription";

            var workoutActivity = new WorkoutActivity
            {
                Id = testActivityId,
                Name = testInitialName,
                Description = testInitialDescription,
                Icon = "ActivityIconUrl",
            };

            await this.workoutActivitiesRepository.AddAsync(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivityEditInputModel = new WorkoutActivityEditInputModel()
            {
                Id = incorrectActivityId,
                Name = testUpdatedName,
                Description = testUpdatedDescription,
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutActivitiesService.UpdateAsync(workoutActivityEditInputModel));
        }

        [Fact]
        public async Task TestUpdateAsync_WithDuplicateName_ShouldThrowInvOpEx()
        {
            var testActivityId = "testActivityId";
            var testInitialName = "testInitialName";
            var testUpdatedName = "testUpdatedName";
            var testInitialDescription = "testInitialDescription";
            var testUpdatedDescription = "testUpdatedDescription";

            await this.workoutActivitiesRepository.AddAsync(new WorkoutActivity()
            {
                Id = "testId",
                Name = testUpdatedName,
                Description = testUpdatedDescription,
            });

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity
            {
                Id = testActivityId,
                Name = testInitialName,
                Description = testInitialDescription,
                Icon = "ActivityIconUrl",
            };

            await this.workoutActivitiesRepository.AddAsync(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivityEditInputModel = new WorkoutActivityEditInputModel()
            {
                Id = testActivityId,
                Name = testUpdatedName,
                Description = testUpdatedDescription,
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.workoutActivitiesService.UpdateAsync(workoutActivityEditInputModel));
        }

        [Fact]
        public async Task TestDeleteAsync_WithTestData_ShouldDeleteWorkoutActivity()
        {
            var testActivityId = "testActivityId";
            var testName = "testName";
            var testDescription = "testDescription";

            await this.workoutActivitiesRepository.AddAsync(new WorkoutActivity()
            {
                Id = testActivityId,
                Name = testName,
                Description = testDescription,
            });

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var initialResult = await this.workoutActivitiesRepository.All()
                .ToArrayAsync();

            Assert.NotEmpty(initialResult);

            await this.workoutActivitiesService.DeleteAsync(testActivityId);

            var finalResult = await this.workoutActivitiesRepository.All()
                .ToArrayAsync();

            Assert.Empty(finalResult);
        }

        [Fact]
        public async Task TestDeleteAsync_WithIncorrectId_ShouldThrowNullRefEx()
        {
            var testActivityId = "testActivityId";
            var incorrectActivityId = "incorrectActivityId";
            var testName = "testName";
            var testDescription = "testDescription";

            await this.workoutActivitiesRepository.AddAsync(new WorkoutActivity()
            {
                Id = testActivityId,
                Name = testName,
                Description = testDescription,
            });

            await this.workoutActivitiesRepository.SaveChangesAsync();

            var initialResult = await this.workoutActivitiesRepository.All()
                .ToArrayAsync();

            Assert.NotEmpty(initialResult);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutActivitiesService.DeleteAsync(incorrectActivityId));
        }
    }
}
