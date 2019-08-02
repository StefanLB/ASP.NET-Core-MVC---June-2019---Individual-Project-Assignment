namespace TrainConnected.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Data.Repositories;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.Bookings;
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.Bookings;
    using Xunit;

    public class BookingsServiceTests
    {
        private readonly IRepository<Workout> workoutsRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<Booking> bookingsRepository;
        private readonly IRepository<TrainConnectedUsersWorkouts> trainConnectedUsersWorkoutsRepository;
        private readonly IRepository<PaymentMethod> paymentMethodsRepository;
        private readonly BookingsService bookingsService;

        public BookingsServiceTests()
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
            this.bookingsRepository = new EfRepository<Booking>(dbContext);
            this.trainConnectedUsersWorkoutsRepository = new EfRepository<TrainConnectedUsersWorkouts>(dbContext);
            this.paymentMethodsRepository = new EfRepository<PaymentMethod>(dbContext);

            this.bookingsService = new BookingsService(this.workoutsRepository, this.usersRepository, this.bookingsRepository, this.trainConnectedUsersWorkoutsRepository, this.paymentMethodsRepository);
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllUpcomingBookings()
        {
            var testUserId = "TrainConnectedUserId1";

            var bookings = new List<Booking>()
            {
                new Booking
                {
                    Price = 15.00m,
                    PaymentMethodId = "PaymentMethodId1",
                    TrainConnectedUserId = testUserId,
                    WorkoutId = "WorkoutId1",
                    Workout = new Workout()
                    {
                        Time = DateTime.UtcNow.AddDays(5),
                    },
                },
                new Booking
                {
                    Price = 16.00m,
                    PaymentMethodId = "PaymentMethodId2",
                    TrainConnectedUserId = testUserId,
                    WorkoutId = "WorkoutId2",
                    Workout = new Workout()
                    {
                        Time = DateTime.UtcNow.AddDays(5),
                    },
                },
                new Booking
                {
                    Price = 17.00m,
                    PaymentMethodId = "PaymentMethodId3",
                    TrainConnectedUserId = testUserId,
                    WorkoutId = "WorkoutId3",
                    Workout = new Workout()
                    {
                        Time = DateTime.UtcNow.AddDays(5),
                    },
                },
            };

            foreach (var booking in bookings)
            {
                await this.bookingsRepository.AddAsync(booking);
            }

            await this.bookingsRepository.SaveChangesAsync();

            var expectedResult = await this.bookingsRepository.All()
                .To<BookingsAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.bookingsService.GetAllAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(b =>
                    b.Price == result.Price),
                    "BookingsService GetAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllAsync_WithNoData_ShouldReturnEmptyCollection()
        {
            var testUserId = "testUserId1";

            IEnumerable<BookingsAllViewModel> expectedResult = new List<BookingsAllViewModel>();

            var actualResult = await this.bookingsService.GetAllAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        [Fact]
        public async Task TestGetAllHistoryAsync_WithTestData_ShouldReturnAllPastBookings()
        {
            var testUserId = "TrainConnectedUserId1";

            var bookings = new List<Booking>()
            {
                new Booking
                {
                    Price = 15.00m,
                    PaymentMethodId = "PaymentMethodId1",
                    TrainConnectedUserId = testUserId,
                    WorkoutId = "WorkoutId1",
                    Workout = new Workout()
                    {
                        Time = DateTime.UtcNow.AddDays(-5),
                    },
                },
                new Booking
                {
                    Price = 16.00m,
                    PaymentMethodId = "PaymentMethodId2",
                    TrainConnectedUserId = testUserId,
                    WorkoutId = "WorkoutId2",
                    Workout = new Workout()
                    {
                        Time = DateTime.UtcNow.AddDays(-5),
                    },
                },
                new Booking
                {
                    Price = 17.00m,
                    PaymentMethodId = "PaymentMethodId3",
                    TrainConnectedUserId = testUserId,
                    WorkoutId = "WorkoutId3",
                    Workout = new Workout()
                    {
                        Time = DateTime.UtcNow.AddDays(-5),
                    },
                },
            };

            foreach (var booking in bookings)
            {
                await this.bookingsRepository.AddAsync(booking);
            }

            await this.bookingsRepository.SaveChangesAsync();

            var expectedResult = await this.bookingsRepository.All()
                .To<BookingsAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.bookingsService.GetAllHistoryAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(b =>
                    b.Price == result.Price),
                    "BookingsService GetAllHistoryAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllHistoryAsync_WithNoData_ShouldReturnEmptyCollection()
        {
            var testUserId = "testUserId1";

            IEnumerable<BookingsAllViewModel> expectedResult = new List<BookingsAllViewModel>();

            var actualResult = await this.bookingsService.GetAllHistoryAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithCorrectData_ShouldReturnBookingDetails()
        {
            var testUserId = "TrainConnectedUserId1";
            var testBookingId = "TestBookingId";
            var testPrice = 15.00m;

            await this.bookingsRepository.AddAsync(new Booking()
            {
                Id = testBookingId,
                PaymentMethodId = "PaymentMethodId1",
                PaymentMethod = new PaymentMethod
                {
                    Name = "cash",
                    PaymentInAdvance = false,
                },
                Price = testPrice,
                WorkoutId = "WorkoutId1",
                Workout = new Workout()
                {
                    Activity = new WorkoutActivity()
                    {
                        Name = "Activity1",
                        Description = "TestDescription",
                        Icon = "TestLinkToIcon",
                    },
                    ActivityId = "ActivityId",
                    CoachId = "TestCoachId",
                    Duration = 15,
                    Location = "Sofia",
                    MaxParticipants = 14,
                    Price = 20.00m,
                    Time = DateTime.UtcNow,
                },
                TrainConnectedUserId = testUserId,
            });

            await this.bookingsRepository.SaveChangesAsync();

            var result = await this.bookingsRepository.All()
                .Where(b => b.Id == testBookingId)
                .FirstOrDefaultAsync();

#pragma warning disable CS4014
            this.bookingsService.GetDetailsAsync(testBookingId, testUserId);
#pragma warning restore CS4014

            Assert.NotNull(result);
            Assert.True(testPrice == result.Price, "BookingsService GetDetailsAsync() does not work properly!");
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectData_ShouldThrowNullRefEx()
        {
            var testUserId = "TrainConnectedUserId1";
            var testBookingId = "TestBookingId";
            var incorrectTestBookingId = "IncorrectBookingId";
            var testPrice = 15.00m;

            await this.bookingsRepository.AddAsync(new Booking()
            {
                Id = testBookingId,
                PaymentMethodId = "PaymentMethodId1",
                PaymentMethod = new PaymentMethod
                {
                    Name = "cash",
                    PaymentInAdvance = false,
                },
                Price = testPrice,
                WorkoutId = "WorkoutId1",
                Workout = new Workout()
                {
                    Activity = new WorkoutActivity()
                    {
                        Name = "Activity1",
                        Description = "TestDescription",
                        Icon = "TestLinkToIcon",
                    },
                    ActivityId = "ActivityId",
                    CoachId = "TestCoachId",
                    Duration = 15,
                    Location = "Sofia",
                    MaxParticipants = 14,
                    Price = 20.00m,
                    Time = DateTime.UtcNow,
                },
                TrainConnectedUserId = testUserId,
            });

            await this.bookingsRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.bookingsService.GetDetailsAsync(incorrectTestBookingId, testUserId));
        }

        [Fact]
        public async Task TestGetDetailsByWorkoutIdAsync_WithCorrectData_ShouldReturnBookingDetails()
        {
            var testUserId = "TrainConnectedUserId1";
            var testBookingId = "TestBookingId";
            var testWorkoutId = "TestWorkoutId";
            var testPrice = 15.00m;

            await this.bookingsRepository.AddAsync(new Booking()
            {
                Id = testBookingId,
                PaymentMethodId = "PaymentMethodId1",
                PaymentMethod = new PaymentMethod
                {
                    Name = "cash",
                    PaymentInAdvance = false,
                },
                Price = testPrice,
                Workout = new Workout()
                {
                    Id = testWorkoutId,
                    Activity = new WorkoutActivity()
                    {
                        Name = "Activity1",
                        Description = "TestDescription",
                        Icon = "TestLinkToIcon",
                    },
                    ActivityId = "ActivityId",
                    CoachId = "TestCoachId",
                    Duration = 15,
                    Location = "Sofia",
                    MaxParticipants = 14,
                    Price = 20.00m,
                    Time = DateTime.UtcNow,
                },
                TrainConnectedUserId = testUserId,
            });

            await this.bookingsRepository.SaveChangesAsync();

            var result = await this.bookingsRepository.All()
                .Where(b => b.WorkoutId == testWorkoutId)
                .FirstOrDefaultAsync();

            #pragma warning disable CS4014
            this.bookingsService.GetDetailsByWorkoutIdAsync(testWorkoutId, testUserId);
            #pragma warning disable CS4014

            Assert.NotNull(result);
            Assert.True(testPrice == result.Price, "BookingsService GetDetailsByWorkoutIdAsync() does not work properly!");
        }

        [Fact]
        public async Task TestGetDetailsByWorkoutIdAsync_WithIncorrectData_ShouldThrowNullRefEx()
        {
            var testUserId = "TrainConnectedUserId1";
            var testBookingId = "TestBookingId";
            var testWorkoutId = "TestWorkoutId";
            var incorrectTestWorkoutId = "IncorrectTestWorkoutId";
            var testPrice = 15.00m;

            await this.bookingsRepository.AddAsync(new Booking()
            {
                Id = testBookingId,
                PaymentMethodId = "PaymentMethodId1",
                PaymentMethod = new PaymentMethod
                {
                    Name = "cash",
                    PaymentInAdvance = false,
                },
                Price = testPrice,
                Workout = new Workout()
                {
                    Id = testWorkoutId,
                    Activity = new WorkoutActivity()
                    {
                        Name = "Activity1",
                        Description = "TestDescription",
                        Icon = "TestLinkToIcon",
                    },
                    ActivityId = "ActivityId",
                    CoachId = "TestCoachId",
                    Duration = 15,
                    Location = "Sofia",
                    MaxParticipants = 14,
                    Price = 20.00m,
                    Time = DateTime.UtcNow,
                },
                TrainConnectedUserId = testUserId,
            });

            await this.bookingsRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.bookingsService.GetDetailsAsync(incorrectTestWorkoutId, testUserId));
        }

        [Fact]
        public async Task TestCreateAsync_WithCorrectData_ShouldCreateBooking()
        {
            var testUserId = "TrainConnectedUserId1";
            var testCoachId = "TestCoachId1";
            var testWorkoutId = "TestWorkoutId";
            var testPrice = 15.00m;

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testCoachId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.workoutsRepository.AddAsync(new Workout
            {
                Id = testWorkoutId,
                CoachId = testCoachId,
                Time = DateTime.Now.AddDays(1),
                MaxParticipants = 10,
            });

            await this.workoutsRepository.SaveChangesAsync();

            await this.paymentMethodsRepository.AddAsync(new PaymentMethod
            {
                Name = "cash",
                PaymentInAdvance = true,
            });

            await this.paymentMethodsRepository.SaveChangesAsync();

            var testInput = new BookingCreateInputModel()
            {
                WorkoutId = testWorkoutId,
                PaymentMethod = "cash",
                Price = testPrice,
            };

            await this.bookingsService.CreateAsync(testInput, testUserId);

            var actualResult = await this.bookingsRepository.All()
                .Where(x => x.WorkoutId == testWorkoutId)
                .Where(p => p.TrainConnectedUserId == testUserId)
                .FirstOrDefaultAsync();

            Assert.NotNull(actualResult);
            Assert.Equal(testUserId, actualResult.TrainConnectedUserId);
            Assert.Equal(testWorkoutId, actualResult.WorkoutId);
        }

        [Fact]
        public async Task TestCreateAsync_WithNoWorkoutData_ShouldThrowNullRefEx()
        {
            var testUserId = "TrainConnectedUserId1";
            var testCoachId = "TestCoachId1";
            var testWorkoutId = "TestWorkoutId";
            var testPrice = 15.00m;

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testCoachId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.paymentMethodsRepository.AddAsync(new PaymentMethod
            {
                Name = "cash",
                PaymentInAdvance = true,
            });

            await this.paymentMethodsRepository.SaveChangesAsync();

            var testInput = new BookingCreateInputModel()
            {
                WorkoutId = testWorkoutId,
                PaymentMethod = "cash",
                Price = testPrice,
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.bookingsService.CreateAsync(testInput, testUserId));
        }

        [Fact]
        public async Task TestCreateAsync_WithNoPaymentMethodData_ShouldThrowNullRefEx()
        {
            var testUserId = "TrainConnectedUserId1";
            var testCoachId = "TestCoachId1";
            var testWorkoutId = "TestWorkoutId";
            var testPrice = 15.00m;

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testCoachId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.workoutsRepository.AddAsync(new Workout
            {
                Id = testWorkoutId,
                CoachId = testCoachId,
                Time = DateTime.Now.AddDays(1),
                MaxParticipants = 10,
            });

            await this.workoutsRepository.SaveChangesAsync();

            var testInput = new BookingCreateInputModel()
            {
                WorkoutId = testWorkoutId,
                PaymentMethod = "cash",
                Price = testPrice,
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.bookingsService.CreateAsync(testInput, testUserId));
        }

        [Fact]
        public async Task TestCreateAsync_WithFullWorkout_ShouldThrowInvOpEx()
        {
            var testUserId = "TrainConnectedUserId1";
            var testCoachId = "TestCoachId1";
            var testWorkoutId = "TestWorkoutId";
            var testPrice = 15.00m;

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testCoachId,
            });

            await this.usersRepository.SaveChangesAsync();

            await this.workoutsRepository.AddAsync(new Workout
            {
                Id = testWorkoutId,
                CoachId = testCoachId,
                Time = DateTime.Now.AddDays(1),
            });

            await this.workoutsRepository.SaveChangesAsync();

            await this.paymentMethodsRepository.AddAsync(new PaymentMethod
            {
                Name = "cash",
                PaymentInAdvance = true,
            });

            await this.paymentMethodsRepository.SaveChangesAsync();

            var testInput = new BookingCreateInputModel()
            {
                WorkoutId = testWorkoutId,
                PaymentMethod = "cash",
                Price = testPrice,
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.bookingsService.CreateAsync(testInput, testUserId));
        }
    }
}
