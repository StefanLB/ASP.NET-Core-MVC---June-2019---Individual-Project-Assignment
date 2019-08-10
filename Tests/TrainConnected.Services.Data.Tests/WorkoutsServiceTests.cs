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
    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.Workouts;
    using Xunit;

    public class WorkoutsServiceTests
    {
        private readonly IRepository<Workout> workoutsRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<WorkoutActivity> workoutActivityRepository;
        private readonly IRepository<TrainConnectedUsersWorkouts> usersWorkoutsRepository;
        private readonly IRepository<PaymentMethod> paymentMethodsRepository;
        private readonly IRepository<WorkoutsPaymentMethods> workoutsPaymentsMethodsRepository;

        private readonly WorkoutsService workoutsService;

        public WorkoutsServiceTests()
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

            this.workoutsRepository = new EfRepository<Workout>(dbContext);
            this.usersRepository = new EfRepository<TrainConnectedUser>(dbContext);
            this.workoutActivityRepository = new EfRepository<WorkoutActivity>(dbContext);
            this.usersWorkoutsRepository = new EfRepository<TrainConnectedUsersWorkouts>(dbContext);
            this.paymentMethodsRepository = new EfRepository<PaymentMethod>(dbContext);
            this.workoutsPaymentsMethodsRepository = new EfRepository<WorkoutsPaymentMethods>(dbContext);

            this.workoutsService = new WorkoutsService(this.workoutsRepository, this.usersRepository, this.workoutActivityRepository, this.usersWorkoutsRepository, this.paymentMethodsRepository, this.workoutsPaymentsMethodsRepository);
        }

        [Fact]
        public async Task TestGetAllUpcomingHomeAsync_WithTestData_ShouldReturnAllUpcomingWorkouts()
        {
            var workouts = new List<Workout>()
            {
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId1",
                        Name = "activityName1",
                        Icon = "iconUrl1",
                    },
                    ActivityId = "activityId1",
                    CoachId = "coachId1",
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId1",
                        UserName = "coachName1",
                    },
                    Duration = 15,
                    Id = "workoutId1",
                    Location = "location1",
                    MaxParticipants = 10,
                    Price = 10.50m,
                    Time = DateTime.Now.AddDays(1),
                },
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId2",
                        Name = "activityName2",
                        Icon = "iconUrl2",
                    },
                    ActivityId = "activityId2",
                    CoachId = "coachId2",
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId2",
                        UserName = "coachName2",
                    },
                    Duration = 16,
                    Id = "workoutId2",
                    Location = "location2",
                    MaxParticipants = 11,
                    Price = 10.51m,
                    Time = DateTime.Now.AddDays(2),
                },
            };

            foreach (var workout in workouts)
            {
                await this.workoutsRepository.AddAsync(workout);
            }

            await this.workoutsRepository.SaveChangesAsync();

            var expectedResult = await this.workoutsRepository.All()
                .Where(t => t.Time > DateTime.UtcNow.ToLocalTime())
                .To<WorkoutsHomeViewModel>()
                .ToArrayAsync();

            var actualResult = await this.workoutsService.GetAllUpcomingHomeAsync();

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(w =>
                    w.Id == result.Id
                    && w.Time == result.Time && w.Location == result.Location),
                    "WorkoutsService GetAllUpcomingHomeAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllUpcomingAsync_WithTestData_ShouldReturnAllUpcomingWorkouts()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workouts = new List<Workout>()
            {
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId1",
                        Name = "activityName1",
                        Icon = "iconUrl1",
                    },
                    ActivityId = "activityId1",
                    CoachId = "coachId1",
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId1",
                        UserName = "coachName1",
                    },
                    Duration = 15,
                    Id = "workoutId1",
                    Location = "location1",
                    MaxParticipants = 10,
                    Price = 10.50m,
                    Time = DateTime.Now.AddDays(-1),
                },
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId2",
                        Name = "activityName2",
                        Icon = "iconUrl2",
                    },
                    ActivityId = "activityId2",
                    CoachId = "coachId2",
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId2",
                        UserName = "coachName2",
                    },
                    Duration = 16,
                    Id = "workoutId2",
                    Location = "location2",
                    MaxParticipants = 11,
                    Price = 10.51m,
                    Time = DateTime.Now.AddDays(2),
                },
            };

            foreach (var workout in workouts)
            {
                await this.workoutsRepository.AddAsync(workout);
            }

            await this.workoutsRepository.SaveChangesAsync();

            var userWorkoutsIds = await this.usersWorkoutsRepository.All()
                .Where(x => x.TrainConnectedUserId == testUserId)
                .Select(w => w.WorkoutId)
                .ToArrayAsync();

            var expectedResult = await this.workoutsRepository.All()
                .Where(t => t.Time > DateTime.UtcNow.ToLocalTime())
                .Where(w => !userWorkoutsIds.Contains(w.Id))
                .To<WorkoutsAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.workoutsService.GetAllUpcomingAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(w =>
                    w.Id == result.Id
                    && w.Time == result.Time && w.Location == result.Location),
                    "WorkoutsService GetAllUpcomingAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetMyAsync_WithTestData_ShouldReturnAllWorkoutsUserSignedUpFor()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workouts = new List<Workout>()
            {
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId1",
                        Name = "activityName1",
                        Icon = "iconUrl1",
                    },
                    ActivityId = "activityId1",
                    CoachId = "coachId1",
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId1",
                        UserName = "coachName1",
                    },
                    Duration = 15,
                    Id = "workoutId1",
                    Location = "location1",
                    MaxParticipants = 10,
                    Price = 10.50m,
                    Time = DateTime.Now.AddDays(-1),
                },
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId2",
                        Name = "activityName2",
                        Icon = "iconUrl2",
                    },
                    ActivityId = "activityId2",
                    CoachId = "coachId2",
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId2",
                        UserName = "coachName2",
                    },
                    Duration = 16,
                    Id = "workoutId2",
                    Location = "location2",
                    MaxParticipants = 11,
                    Price = 10.51m,
                    Time = DateTime.Now.AddDays(2),
                },
            };

            foreach (var workout in workouts)
            {
                await this.workoutsRepository.AddAsync(workout);
            }

            await this.workoutsRepository.SaveChangesAsync();

            await this.usersWorkoutsRepository.AddAsync(new TrainConnectedUsersWorkouts()
            {
                TrainConnectedUserId = testUserId,
                TrainConnectedUser = user,
                WorkoutId = "workoutId1",
                Workout = workouts.Where(w => w.Id == "workoutId1").FirstOrDefault(),
            });

            await this.usersWorkoutsRepository.SaveChangesAsync();

            var userWorkoutsIds = await this.usersWorkoutsRepository.All()
                .Where(x => x.TrainConnectedUserId == testUserId)
                .Select(w => w.WorkoutId)
                .ToArrayAsync();

            var expectedResult = await this.workoutsRepository.All()
                .Where(w => userWorkoutsIds.Contains(w.Id))
                .To<WorkoutsAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.workoutsService.GetMyAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(w =>
                    w.Id == result.Id
                    && w.Time == result.Time && w.Location == result.Location),
                    "WorkoutsService GetMyAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetMyCreatedAsync_WithTestData_ShouldReturnAllWorkoutsCreatedByUser()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workouts = new List<Workout>()
            {
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId1",
                        Name = "activityName1",
                        Icon = "iconUrl1",
                    },
                    ActivityId = "activityId1",
                    CoachId = testUserId,
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId1",
                        UserName = "coachName1",
                    },
                    Duration = 15,
                    Id = "workoutId1",
                    Location = "location1",
                    MaxParticipants = 10,
                    Price = 10.50m,
                    Time = DateTime.Now.AddDays(-1),
                },
                new Workout
                {
                    Activity = new WorkoutActivity()
                    {
                        Id = "activityId2",
                        Name = "activityName2",
                        Icon = "iconUrl2",
                    },
                    ActivityId = "activityId2",
                    CoachId = "coachId2",
                    Coach = new TrainConnectedUser()
                    {
                        Id = "coachId2",
                        UserName = "coachName2",
                    },
                    Duration = 16,
                    Id = "workoutId2",
                    Location = "location2",
                    MaxParticipants = 11,
                    Price = 10.51m,
                    Time = DateTime.Now.AddDays(2),
                },
            };

            foreach (var workout in workouts)
            {
                await this.workoutsRepository.AddAsync(workout);
            }

            await this.workoutsRepository.SaveChangesAsync();

            var expectedResult = await this.workoutsRepository.All()
                .Where(w => w.CoachId == testUserId)
                .To<WorkoutsAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.workoutsService.GetMyCreatedAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(w =>
                    w.Id == result.Id
                    && w.Time == result.Time && w.Location == result.Location),
                    "WorkoutsService GetMyCreatedAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestCreateAsync_WithTestData_ShouldCreateWorkout()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(-1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            var actualResult = await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var expectedResult = await this.workoutsRepository.All()
                .Where(w => w.CoachId == testUserId)
                .To<WorkoutDetailsViewModel>()
                .FirstOrDefaultAsync();

            Assert.Equal(expectedResult.ActivityName, actualResult.ActivityName);
            Assert.Equal(expectedResult.Duration, actualResult.Duration);
            Assert.Equal(expectedResult.Price, actualResult.Price);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.MaxParticipants, actualResult.MaxParticipants);
            Assert.Equal(expectedResult.Time, actualResult.Time);
            Assert.Equal(expectedResult.Id, actualResult.Id);
        }

        [Fact]
        public async Task TestCreateAsync_WithMissingActivityId_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            // await this.workoutActivityRepository.AddAsync(workoutActivity);
            // await this.workoutActivityRepository.SaveChangesAsync();
            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(-1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId));
        }

        [Fact]
        public async Task TestCreateAsync_WithMissingPaymentMethod_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            // await this.paymentMethodsRepository.AddAsync(paymentMethod);
            // await this.paymentMethodsRepository.SaveChangesAsync();
            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(-1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId));
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithTestData_ShouldReturnWorkoutDetails()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var expectedResult = await this.workoutsRepository.All()
                .Where(w => w.CoachId == testUserId)
                .To<WorkoutDetailsViewModel>()
                .FirstOrDefaultAsync();

            var workoutId = expectedResult.Id;
            var testSecondUserid = "testUseridForBooking";

            var actualResult = await this.workoutsService.GetDetailsAsync(workoutId, testSecondUserid);

            Assert.Equal(expectedResult.ActivityName, actualResult.ActivityName);
            Assert.Equal(expectedResult.Duration, actualResult.Duration);
            Assert.Equal(expectedResult.Price, actualResult.Price);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.MaxParticipants, actualResult.MaxParticipants);
            Assert.Equal(expectedResult.Time, actualResult.Time);
            Assert.Equal(expectedResult.Id, actualResult.Id);
            Assert.True(actualResult.IsBookableByUser);
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectWorkoutId_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var incorrectWorkoutId = "incorrectWorkoutId";

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutsService.GetDetailsAsync(incorrectWorkoutId, testUserId));
        }

        [Fact]
        public async Task TestGetCancelDetailsAsync_WithTestData_ShouldReturnWorkoutDetails()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var expectedResult = await this.workoutsRepository.All()
                .Where(w => w.CoachId == testUserId)
                .To<WorkoutCancelViewModel>()
                .FirstOrDefaultAsync();

            var workoutId = expectedResult.Id;

            var actualResult = await this.workoutsService.GetCancelDetailsAsync(workoutId, testUserId);

            Assert.Equal(expectedResult.ActivityName, actualResult.ActivityName);
            Assert.Equal(expectedResult.Duration, actualResult.Duration);
            Assert.Equal(expectedResult.Price, actualResult.Price);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.MaxParticipants, actualResult.MaxParticipants);
            Assert.Equal(expectedResult.Time, actualResult.Time);
            Assert.Equal(expectedResult.Id, actualResult.Id);
        }

        [Fact]
        public async Task TestGetCancelDetailsAsync_WithIncorrectWorkoutId_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var incorrectWorkoutId = "incorrectWorkoutId";

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutsService.GetCancelDetailsAsync(incorrectWorkoutId, testUserId));
        }

        [Fact]
        public async Task TestGetCancelDetailsAsync_WithIncorrectCoachId_ShouldThrowArgEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var expectedResult = await this.workoutsRepository.All()
                .Where(w => w.CoachId == testUserId)
                .To<WorkoutCancelViewModel>()
                .FirstOrDefaultAsync();

            var workoutId = expectedResult.Id;
            var incorrectCoachId = "incorrectCoachId";

            await Assert.ThrowsAsync<ArgumentException>(async () => await this.workoutsService.GetCancelDetailsAsync(workoutId, incorrectCoachId));
        }

        [Fact]
        public async Task TestCancelAsync_WithTestData_ShouldCancelWorkout()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var initialResult = await this.workoutsRepository.All()
                .ToArrayAsync();

            Assert.NotEmpty(initialResult);

            var workoutId = initialResult.FirstOrDefault().Id;

            await this.workoutsService.CancelAsync(workoutId, testUserId);

            var finalResult = await this.workoutsRepository.All()
                .ToArrayAsync();

            Assert.Empty(finalResult);
        }

        [Fact]
        public async Task TestCancelAsync_WithIncorrectWorkoutId_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var createdWorkout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync();

            var incorrectWorkoutId = "incorrect" + createdWorkout.Id;

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.workoutsService.CancelAsync(incorrectWorkoutId, testUserId));
        }

        [Fact]
        public async Task TestCancelAsync_WithIncorrectCoachId_ShouldThrowArgEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var createdWorkout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync();

            var workoutId = createdWorkout.Id;
            var incorrectCoachId = "incorrectCoachId";

            await Assert.ThrowsAsync<ArgumentException>(async () => await this.workoutsService.CancelAsync(workoutId, incorrectCoachId));
        }

        [Fact]
        public async Task TestCancelAsync_WithAlreadyStartedWorkout_ShouldThrowInvOpEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var workoutActivity = new WorkoutActivity()
            {
                Id = "activityId1",
                Name = "activityName1",
                Icon = "iconUrl1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var paymentMethod = new PaymentMethod()
            {
                Id = "activityId1",
                Name = "activityName1",
                PaymentInAdvance = false,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            WorkoutCreateInputModel workoutCreateInputModel = new WorkoutCreateInputModel()
            {
                Activity = workoutActivity.Name,
                Duration = 15,
                Location = "location1",
                MaxParticipants = 10,
                Price = 10.50m,
                Time = DateTime.Now.AddDays(-1),
                PaymentMethods = new List<string>() { paymentMethod.Name },
            };

            await this.workoutsService.CreateAsync(workoutCreateInputModel, testUserId);

            var createdWorkout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync();

            var workoutId = createdWorkout.Id;

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.workoutsService.CancelAsync(workoutId, testUserId));
        }
    }
}
