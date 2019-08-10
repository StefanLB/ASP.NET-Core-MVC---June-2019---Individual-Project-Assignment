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
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Data.Repositories;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.Withdrawals;
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.Withdrawals;
    using Xunit;

    public class WithdrawalsServiceTests
    {
        private readonly IRepository<Withdrawal> withdrawalsRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly WithdrawalsService withdrawalsService;

        public WithdrawalsServiceTests()
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

            this.withdrawalsRepository = new EfRepository<Withdrawal>(dbContext);
            this.usersRepository = new EfRepository<TrainConnectedUser>(dbContext);
            this.withdrawalsService = new WithdrawalsService(this.withdrawalsRepository, this.usersRepository);
        }

        [Fact]
        public async Task TestGetAllAdminAsync_WithTestData_ShouldReturnAllWithdrawals()
        {
            var withdrawals = new List<Withdrawal>()
            {
                new Withdrawal
                {
                    Amount = 10.05m,
                    Id = "testWithdrawalId1",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = "testUserId1",
                },
                new Withdrawal
                {
                    Amount = 10.05m,
                    Id = "testWithdrawalId2",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = "testUserId2",
                },
                new Withdrawal
                {
                    Amount = 10.05m,
                    Id = "testWithdrawalId3",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = "testUserId3",
                },
            };

            foreach (var w in withdrawals)
            {
                await this.withdrawalsRepository.AddAsync(w);
            }

            await this.withdrawalsRepository.SaveChangesAsync();

            var expectedResult = await this.withdrawalsRepository.All()
                .To<WithdrawalsProcessingViewModel>()
                .ToArrayAsync();

            var actualResult = await this.withdrawalsService.GetAllAdminAsync();

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(w =>
                    w.TrainConnectedUserId == result.TrainConnectedUserId
                    && w.Id == result.Id),
                    "WithdrawalsService GetAllAdminAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestProcessAsync_WithTestData_ShouldProcessWithdrawal()
        {
            var testUserId = "testUserId";
            var testAdminId = "testAdminId";
            var testWithdrawalId = "testWithdrawalId";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 100.00m,
            };

            var adminUser = new TrainConnectedUser()
            {
                Id = testAdminId,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.AddAsync(adminUser);
            await this.usersRepository.SaveChangesAsync();

            var withdrawal = new Withdrawal()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                CreatedOn = DateTime.Now,
                TrainConnectedUserId = testUserId,
            };

            await this.withdrawalsRepository.AddAsync(withdrawal);
            await this.withdrawalsRepository.SaveChangesAsync();

            var inputModel = new WithdrawalProcessInputModel()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                Status = true,
                TrainConnectedUserId = testUserId,
            };

            await this.withdrawalsService.ProcessAsync(inputModel, testAdminId);

            var expectedWithdrawalStatus = "Approved";

            var actualWithdrawalStatus = await this.withdrawalsRepository.All()
                .Where(w => w.Id == testWithdrawalId)
                .Select(s => s.Status.ToString())
                .FirstOrDefaultAsync();

            Assert.Equal(expectedWithdrawalStatus, actualWithdrawalStatus);
        }

        [Fact]
        public async Task TestProcessAsync_WithIncorrectWithdrawal_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testAdminId = "testAdminId";
            var testWithdrawalId = "testWithdrawalId";
            var incorrectWithdrawalId = "incorrectWithdrawalId";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 100.00m,
            };

            var adminUser = new TrainConnectedUser()
            {
                Id = testAdminId,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.AddAsync(adminUser);
            await this.usersRepository.SaveChangesAsync();

            var withdrawal = new Withdrawal()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                CreatedOn = DateTime.Now,
                TrainConnectedUserId = testUserId,
            };

            await this.withdrawalsRepository.AddAsync(withdrawal);
            await this.withdrawalsRepository.SaveChangesAsync();

            var inputModel = new WithdrawalProcessInputModel()
            {
                Amount = 10.05m,
                Id = incorrectWithdrawalId,
                Status = true,
                TrainConnectedUserId = testUserId,
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.withdrawalsService.ProcessAsync(inputModel, testAdminId));
        }

        [Fact]
        public async Task TestProcessAsync_WithIncorrectUser_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var incorrectTestUserId = "incorrectTestUserId";
            var testAdminId = "testAdminId";
            var testWithdrawalId = "testWithdrawalId";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 100.00m,
            };

            var adminUser = new TrainConnectedUser()
            {
                Id = testAdminId,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.AddAsync(adminUser);
            await this.usersRepository.SaveChangesAsync();

            var withdrawal = new Withdrawal()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                CreatedOn = DateTime.Now,
                TrainConnectedUserId = incorrectTestUserId,
            };

            await this.withdrawalsRepository.AddAsync(withdrawal);
            await this.withdrawalsRepository.SaveChangesAsync();

            var inputModel = new WithdrawalProcessInputModel()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                Status = true,
                TrainConnectedUserId = incorrectTestUserId,
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.withdrawalsService.ProcessAsync(inputModel, testAdminId));
        }

        [Fact]
        public async Task TestProcessAsync_WithStatusRejected_ShouldFinalizeWithdrawal()
        {
            var testUserId = "testUserId";
            var testAdminId = "testAdminId";
            var testWithdrawalId = "testWithdrawalId";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 100.00m,
            };

            var adminUser = new TrainConnectedUser()
            {
                Id = testAdminId,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.AddAsync(adminUser);
            await this.usersRepository.SaveChangesAsync();

            var withdrawal = new Withdrawal()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                CreatedOn = DateTime.Now,
                TrainConnectedUserId = testUserId,
            };

            await this.withdrawalsRepository.AddAsync(withdrawal);
            await this.withdrawalsRepository.SaveChangesAsync();

            var inputModel = new WithdrawalProcessInputModel()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                Status = false,
                TrainConnectedUserId = testUserId,
            };

            await this.withdrawalsService.ProcessAsync(inputModel, testAdminId);

            var expectedWithdrawalStatus = "Rejected";

            var actualWithdrawalStatus = await this.withdrawalsRepository.All()
                .Where(w => w.Id == testWithdrawalId)
                .Select(s => s.Status.ToString())
                .FirstOrDefaultAsync();

            Assert.Equal(expectedWithdrawalStatus, actualWithdrawalStatus);
        }

        [Fact]
        public async Task TestGetForProcessingAsync_WithTestData_ShouldReturnWithdrawal()
        {
            var testUserId = "testUserId";
            var testWithdrawalId = "testWithdrawalId";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 100.00m,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var withdrawal = new Withdrawal()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                CreatedOn = DateTime.Now,
                TrainConnectedUserId = testUserId,
                TrainConnectedUser = user,
                Status = StatusCode.Initiated,
            };

            await this.withdrawalsRepository.AddAsync(withdrawal);
            await this.withdrawalsRepository.SaveChangesAsync();

            var expectedResult = await this.withdrawalsRepository.All()
                .To<WithdrawalsProcessingViewModel>()
                .FirstOrDefaultAsync();

            var actualResult = await this.withdrawalsService.GetForProcessingAsync(testWithdrawalId);

            Assert.Equal(expectedResult.TrainConnectedUserId, actualResult.TrainConnectedUserId);
            Assert.Equal(expectedResult.Amount, actualResult.Amount);
        }

        [Fact]
        public async Task TestGetForProcessingAsync_WithIncorrectData_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testWithdrawalId = "testWithdrawalId";
            var incorrectWithdrawalId = "incorrectWithdrawalId";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 100.00m,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var withdrawal = new Withdrawal()
            {
                Amount = 10.05m,
                Id = testWithdrawalId,
                CreatedOn = DateTime.Now,
                TrainConnectedUserId = testUserId,
                TrainConnectedUser = user,
                Status = StatusCode.Initiated,
            };

            await this.withdrawalsRepository.AddAsync(withdrawal);
            await this.withdrawalsRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.withdrawalsService.GetForProcessingAsync(incorrectWithdrawalId));
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllUserWithdrawals()
        {
            var testUserId = "testUserId";

            var withdrawals = new List<Withdrawal>()
            {
                new Withdrawal
                {
                    Amount = 10.05m,
                    Id = "testWithdrawalId1",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = testUserId,
                },
                new Withdrawal
                {
                    Amount = 10.05m,
                    Id = "testWithdrawalId2",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = testUserId,
                },
                new Withdrawal
                {
                    Amount = 10.05m,
                    Id = "testWithdrawalId3",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = testUserId,
                },
            };

            foreach (var w in withdrawals)
            {
                await this.withdrawalsRepository.AddAsync(w);
            }

            await this.withdrawalsRepository.SaveChangesAsync();

            var expectedResult = await this.withdrawalsRepository.All()
                .To<WithdrawalsAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.withdrawalsService.GetAllAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(w =>
                    w.Id == result.Id),
                    "WithdrawalsService GetAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestCreateAsync_WithTestData_ShouldCreateWithdrawal()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var withdrawalCreateInputModel = new WithdrawalCreateInputModel()
            {
                Amount = 15.45m,
                AdditionalInstructions = "NoneOfTheAbove",
            };

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 30.00m,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            await this.withdrawalsService.CreateAsync(withdrawalCreateInputModel, testUserId);

            var result = await this.withdrawalsRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.True(result.Status == StatusCode.Initiated);
        }

        [Fact]
        public async Task TestCreateAsync_WithNegativeAmount_ShouldThrowInvOpEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var withdrawalCreateInputModel = new WithdrawalCreateInputModel()
            {
                Amount = -15.45m,
                AdditionalInstructions = "NoneOfTheAbove",
            };

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = 30.00m,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.withdrawalsService.CreateAsync(withdrawalCreateInputModel, testUserId));
        }

        [Fact]
        public async Task TestGetUserBalanceAsync_WithTestData_ShouldReturnUserBalance()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testUserBalance = 13480037.63m;

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = testUserBalance,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var result = await this.withdrawalsService.GetUserBalanceAsync(testUserId);

            Assert.Equal(testUserBalance, result);
        }

        [Fact]
        public async Task TestGetUserBalanceAsync_WithIncorrectUser_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var incorrectUserId = "incorrectUserId";
            var testUserName = "testUserName";
            var testUserBalance = 13480037.63m;

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = testUserBalance,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.withdrawalsService.GetUserBalanceAsync(incorrectUserId));
        }

        [Fact]
        public async Task TestGetUserPendingWithdrawalsAsync_WithTestData_ShouldReturnTotalPendingUserWithdrawals()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testUserBalance = 13480037.63m;

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = testUserBalance,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var withdrawals = new List<Withdrawal>()
            {
                new Withdrawal
                {
                    Amount = 100.10m,
                    Id = "testWithdrawalId1",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = testUserId,
                    Status = StatusCode.Initiated,
                },
                new Withdrawal
                {
                    Amount = 5000.20m,
                    Id = "testWithdrawalId2",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = "differentUserId",
                    Status = StatusCode.Initiated,
                },
                new Withdrawal
                {
                    Amount = 100.10m,
                    Id = "testWithdrawalId3",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = testUserId,
                    Status = StatusCode.InProcess,
                },
                new Withdrawal
                {
                    Amount = 100.10m,
                    Id = "testWithdrawalId4",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = testUserId,
                    Status = StatusCode.Approved,
                },
            };

            foreach (var w in withdrawals)
            {
                await this.withdrawalsRepository.AddAsync(w);
            }

            await this.withdrawalsRepository.SaveChangesAsync();

            var expectedResultList = await this.withdrawalsRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .Where(s => s.Status == StatusCode.Initiated || s.Status == StatusCode.InProcess)
                .Select(a => a.Amount)
                .ToArrayAsync();

            var expectedResult = expectedResultList.Sum();

            var actualResult = await this.withdrawalsService.GetUserPendingWithdrawalsBalance(testUserId);

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task TestGetUserPendingWithdrawalsAsync_WithIncorrectUser_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var incorrectUserId = "incorrectUserId";
            var testUserName = "testUserName";
            var testUserBalance = 13480037.63m;

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                Balance = testUserBalance,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            var withdrawals = new List<Withdrawal>()
            {
                new Withdrawal
                {
                    Amount = 100.10m,
                    Id = "testWithdrawalId1",
                    CreatedOn = DateTime.Now,
                    TrainConnectedUserId = testUserId,
                    Status = StatusCode.Initiated,
                },
            };

            foreach (var w in withdrawals)
            {
                await this.withdrawalsRepository.AddAsync(w);
            }

            await this.withdrawalsRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.withdrawalsService.GetUserPendingWithdrawalsBalance(incorrectUserId));
        }
    }
}
