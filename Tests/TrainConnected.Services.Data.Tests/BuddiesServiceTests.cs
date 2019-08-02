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
    using TrainConnected.Services.Data;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.Buddies;
    using Xunit;

    public class BuddiesServiceTests
    {
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<TrainConnectedUsersBuddies> usersBuddiesRepository;
        private readonly IRepository<Certificate> certificatesRepository;
        private readonly IRepository<Workout> workoutsRepository;

        private readonly BuddiesService buddiesService;

        public BuddiesServiceTests()
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

            this.usersRepository = new EfRepository<TrainConnectedUser>(dbContext);
            this.usersBuddiesRepository = new EfRepository<TrainConnectedUsersBuddies>(dbContext);
            this.certificatesRepository = new EfRepository<Certificate>(dbContext);
            this.workoutsRepository = new EfRepository<Workout>(dbContext);

            this.buddiesService = new BuddiesService(this.usersRepository, this.usersBuddiesRepository, this.certificatesRepository, this.workoutsRepository);
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllUserBuddies()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            });

            for (int i = 0; i < 10; i++)
            {
                await this.usersRepository.AddAsync(new TrainConnectedUser()
                {
                    Id = i.ToString(),
                    UserName = "buddy" + i.ToString(),
                });

                await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
                {
                    TrainConnectedUserId = testUserId,
                    TrainConnectedBuddyId = "buddy" + i.ToString(),
                    TrainConnectedBuddy = new TrainConnectedUser()
                    {
                        Id = "buddy" + i.ToString(),
                    },
                });
            }

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            var expectedResultIds = await this.usersBuddiesRepository.All()
                .Where(i => i.TrainConnectedUserId == testUserId)
                .ToArrayAsync();

            var actualResult = await this.buddiesService.GetAllAsync(testUserId);
            var actualResultIds = actualResult.Select(i => i.Id);

            Assert.Equal(expectedResultIds.Count(), actualResultIds.Count());

            foreach (var resultId in actualResultIds)
            {
                Assert.True(
                    expectedResultIds.Any(i =>
                    i.TrainConnectedBuddyId == resultId),
                    "BuddiesService GetAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllAsync_WithNoData_ShouldReturnNoBuddies()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            });

            var actualResult = await this.buddiesService.GetAllAsync(testUserId);

            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task TestGetAllAsync_WithDifferentUserId_ShouldReturnNoBuddies()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testDifferentUserId = "testDifferentUserId";

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            });

            for (int i = 0; i < 10; i++)
            {
                await this.usersRepository.AddAsync(new TrainConnectedUser()
                {
                    Id = i.ToString(),
                    UserName = "buddy" + i.ToString(),
                });

                await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
                {
                    TrainConnectedUserId = testDifferentUserId,
                    TrainConnectedBuddyId = "buddy" + i.ToString(),
                    TrainConnectedBuddy = new TrainConnectedUser()
                    {
                        Id = "buddy" + i.ToString(),
                    },
                });
            }

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            var expectedResultIds = await this.usersBuddiesRepository.All()
                .Where(i => i.TrainConnectedUserId == testUserId)
                .ToArrayAsync();

            var actualResult = await this.buddiesService.GetAllAsync(testUserId);

            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task TestFindAllAsync_WithTestData_ShouldReturnAllUserNonBuddies()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testDifferentUserId = "testDifferentUserId";

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            });

            for (int i = 0; i < 5; i++)
            {
                await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
                {
                    TrainConnectedUserId = testDifferentUserId,
                    TrainConnectedBuddyId = "buddy" + i.ToString(),
                    TrainConnectedBuddy = new TrainConnectedUser()
                    {
                        Id = "buddy" + i.ToString(),
                    },
                });
            }

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            var expectedResultIds = await this.usersBuddiesRepository.All()
                .Where(i => i.TrainConnectedUserId != testUserId)
                .ToArrayAsync();

            var actualResult = await this.buddiesService.FindAllAsync(testUserId);
            var actualResultIds = actualResult.Select(i => i.Id);

            Assert.Equal(expectedResultIds.Count(), actualResultIds.Count());

            foreach (var resultId in actualResultIds)
            {
                Assert.True(
                    expectedResultIds.Any(i =>
                    i.TrainConnectedBuddyId == resultId),
                    "BuddiesService FindAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestFindAllAsync_WithBuddyData_ShouldReturnAllUserNonBuddies()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testDifferentUserId = "testDifferentUserId";

            await this.usersRepository.AddAsync(new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            });

            for (int i = 0; i < 5; i++)
            {
                await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
                {
                    TrainConnectedUserId = testUserId,
                    TrainConnectedBuddyId = "buddy" + i.ToString(),
                    TrainConnectedBuddy = new TrainConnectedUser()
                    {
                        Id = "buddy" + i.ToString(),
                    },
                });
            }

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            var expectedResultIds = await this.usersBuddiesRepository.All()
                .Where(i => i.TrainConnectedUserId != testUserId)
                .ToArrayAsync();

            var actualResult = await this.buddiesService.FindAllAsync(testUserId);
            var actualResultIds = actualResult.Select(i => i.Id);

            Assert.Equal(expectedResultIds.Count(), actualResultIds.Count());

            foreach (var resultId in actualResultIds)
            {
                Assert.True(
                    expectedResultIds.Any(i =>
                    i.TrainConnectedBuddyId == resultId),
                    "BuddiesService FindAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithCorrectData_ShouldReturnBuddyDetails()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
            {
                TrainConnectedUserId = testUserId,
                TrainConnectedUser = testUser,
                TrainConnectedBuddyId = testBuddyId,
                TrainConnectedBuddy = testBuddyUser,
            });

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            var expectedResult = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .Where(b => b.TrainConnectedBuddyId == testBuddyId)
                .FirstOrDefaultAsync();

            var actualResult = await this.buddiesService.GetDetailsAsync(testBuddyId, testUserId);

            Assert.Equal(expectedResult.TrainConnectedBuddyId, actualResult.Id);
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectBuddyData_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";
            var incorrectBuddyId = "incorrectBuddyId";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
            {
                TrainConnectedUserId = testUserId,
                TrainConnectedUser = testUser,
                TrainConnectedBuddyId = testBuddyId,
                TrainConnectedBuddy = testBuddyUser,
            });

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            var expectedResult = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .Where(b => b.TrainConnectedBuddyId == testBuddyId)
                .FirstOrDefaultAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.buddiesService.GetDetailsAsync(incorrectBuddyId, testUserId));
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectBuddyData_ShouldThrowArgEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<ArgumentException>(async () => await this.buddiesService.GetDetailsAsync(testBuddyId, testUserId));
        }

        [Fact]
        public async Task TestGetCoachDetailsAsync_WithCorrectData_ShouldReturnCoach()
        {
            var testCoachId = "testCoachId";
            var testCoachUserName = "testCoachUserName";

            var testCoach = new TrainConnectedUser()
            {
                Id = testCoachId,
                UserName = testCoachUserName,
            };

            await this.usersRepository.AddAsync(testCoach);
            await this.usersRepository.SaveChangesAsync();

            var expectedResult = await this.usersRepository.All()
                .Where(x => x.UserName == testCoachUserName)
                .FirstOrDefaultAsync();

            var actualResult = await this.buddiesService.GetCoachDetailsAsync(testCoachUserName);

            Assert.Equal(expectedResult.Id, actualResult.Id);
            Assert.Equal(expectedResult.UserName, actualResult.UserName);
        }

        [Fact]
        public async Task TestGetCoachDetailsAsync_WithInCorrectData_ShouldThrowNullRefEx()
        {
            var testCoachId = "testCoachId";
            var testCoachUserName = "testCoachUserName";
            var incorrectCoachUserName = "incorrectCoachUserName";

            var testCoach = new TrainConnectedUser()
            {
                Id = testCoachId,
                UserName = testCoachUserName,
            };

            await this.usersRepository.AddAsync(testCoach);
            await this.usersRepository.SaveChangesAsync();

            var expectedResult = await this.usersRepository.All()
                .Where(x => x.UserName == testCoachUserName)
                .FirstOrDefaultAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.buddiesService.GetCoachDetailsAsync(incorrectCoachUserName));
        }

        [Fact]
        public async Task TestAddAsync_WithCorrectData_ShouldAddBuddy()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersRepository.SaveChangesAsync();

            await this.buddiesService.AddAsync(testBuddyId, testUserId);

            var result = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .Where(b => b.TrainConnectedBuddyId == testBuddyId)
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(result.TrainConnectedUserId, testUserId);
            Assert.Equal(result.TrainConnectedBuddyId, testBuddyId);
        }

        [Fact]
        public async Task TestAddAsync_WithInCorrectUserId_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";
            var incorrectTestUserId = "incorrectTestUserId";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.buddiesService.AddAsync(testBuddyId, incorrectTestUserId));
        }

        [Fact]
        public async Task TestAddAsync_WithInCorrectBuddyId_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";
            var incorrectBuddyId = "incorrectBuddyId";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.buddiesService.AddAsync(incorrectBuddyId, testUserId));
        }

        [Fact]
        public async Task TestAddAsync_WithSameUserAndBuddyId_ShouldThrowInvOpEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.buddiesService.AddAsync(testUserId, testUserId));
        }

        [Fact]
        public async Task TestRemoveAsync_WithCorrectData_ShouldRemoveBuddy()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
            {
                TrainConnectedUserId = testUserId,
                TrainConnectedUser = testUser,
                TrainConnectedBuddyId = testBuddyId,
                TrainConnectedBuddy = testBuddyUser,
            });

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            await this.buddiesService.RemoveAsync(testBuddyId, testUserId);

            var result = await this.usersBuddiesRepository.All().ToArrayAsync();
            var resultCount = result.Count();

            Assert.Equal(0, resultCount);
        }

        [Fact]
        public async Task TestRemoveAsync_WithIncorrectBuddy_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";
            var incorrectTestBuddyId = "incorrectTestBuddyId";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);

            await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies()
            {
                TrainConnectedUserId = testUserId,
                TrainConnectedUser = testUser,
                TrainConnectedBuddyId = testBuddyId,
                TrainConnectedBuddy = testBuddyUser,
            });

            await this.usersRepository.SaveChangesAsync();
            await this.usersBuddiesRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.buddiesService.RemoveAsync(incorrectTestBuddyId, testUserId));
        }

        [Fact]
        public async Task TestRemoveAsync_WithNoBuddyRelation_ShouldThrowArgEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testBuddyId = "testBuddyId";
            var testBuddyName = "testBuddyName";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(testUser);

            var testBuddyUser = new TrainConnectedUser()
            {
                Id = testBuddyId,
                UserName = testBuddyName,
            };

            await this.usersRepository.AddAsync(testBuddyUser);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<ArgumentException>(async () => await this.buddiesService.RemoveAsync(testBuddyId, testUserId));
        }
    }
}
