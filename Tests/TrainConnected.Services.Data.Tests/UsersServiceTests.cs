namespace TrainConnected.Services.Data.Tests
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using TrainConnected.Data;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Data.Repositories;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.Users;
    using Xunit;

    public class UsersServiceTests
    {
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<IdentityUserRole<string>> usersRolesRepository;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<TrainConnectedUser> userManager;

        private readonly UsersService usersService;

        public UsersServiceTests()
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
            this.usersRolesRepository = new EfRepository<IdentityUserRole<string>>(dbContext);

            this.usersService = new UsersService(this.usersRepository, this.usersRolesRepository, this.roleManager, this.userManager);
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllUsers()
        {
            var adminId = "adminId";

            var users = new List<TrainConnectedUser>()
            {
                new TrainConnectedUser
                {
                    UserName = "TestUserName1",
                    LockoutEnd = DateTime.Now.AddDays(5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName2",
                    LockoutEnd = DateTime.Now.AddDays(-5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName3",
                    LockoutEnd = DateTime.Now,
                },
            };

            foreach (var user in users)
            {
                await this.usersRepository.AddAsync(user);
            }

            await this.usersRepository.SaveChangesAsync();

            var expectedResult = await this.usersRepository.All()
                .To<UsersAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.usersService.GetAllAsync(adminId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(u =>
                    u.UserName == result.UserName
                    && u.Id == result.Id),
                    "UsersService GetAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllAsync_WithNoData_ShouldReturnNoUsers()
        {
            var adminId = "adminId";

            var expectedResult = await this.usersRepository.All()
                .To<UsersAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.usersService.GetAllAsync(adminId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        [Fact]
        public async Task TestUnlockUserAsync_WithTestData_ShouldUnlockUser()
        {
            var userIdToUnlock = "userId1";

            var users = new List<TrainConnectedUser>()
            {
                new TrainConnectedUser
                {
                    Id = userIdToUnlock,
                    UserName = "TestUserName1",
                    LockoutEnd = DateTime.Now.AddDays(5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName2",
                    LockoutEnd = DateTime.Now.AddDays(-5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName3",
                    LockoutEnd = DateTime.Now,
                },
            };

            foreach (var user in users)
            {
                await this.usersRepository.AddAsync(user);
            }

            await this.usersRepository.SaveChangesAsync();

            var resultBeforeChange = await this.usersRepository.All()
                .Where(u => u.Id == userIdToUnlock)
                .FirstOrDefaultAsync();

            Assert.NotNull(resultBeforeChange.LockoutEnd);

            await this.usersService.UnlockUserAsync(userIdToUnlock);

            var resultAfterChange = await this.usersRepository.All()
                .Where(u => u.Id == userIdToUnlock)
                .FirstOrDefaultAsync();

            Assert.Null(resultAfterChange.LockoutEnd);
        }

        [Fact]
        public async Task TestUnlockUserAsync_WithIncorrectUserId_ShouldThrowNullRefEx()
        {
            var userIdToUnlock = "userId1";
            var incorrectUserIdToUnlock = "incorrectUserId1";

            var users = new List<TrainConnectedUser>()
            {
                new TrainConnectedUser
                {
                    Id = userIdToUnlock,
                    UserName = "TestUserName1",
                    LockoutEnd = DateTime.Now.AddDays(5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName2",
                    LockoutEnd = DateTime.Now.AddDays(-5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName3",
                    LockoutEnd = DateTime.Now,
                },
            };

            foreach (var user in users)
            {
                await this.usersRepository.AddAsync(user);
            }

            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.usersService.UnlockUserAsync(incorrectUserIdToUnlock));
        }

        [Fact]
        public async Task TestLockUserAsync_WithTestData_ShouldLockUser()
        {
            var userIdToLock = "userId1";

            var users = new List<TrainConnectedUser>()
            {
                new TrainConnectedUser
                {
                    Id = userIdToLock,
                    UserName = "TestUserName1",
                    LockoutEnd = null,
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName2",
                    LockoutEnd = DateTime.Now.AddDays(-5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName3",
                    LockoutEnd = DateTime.Now,
                },
            };

            foreach (var user in users)
            {
                await this.usersRepository.AddAsync(user);
            }

            await this.usersRepository.SaveChangesAsync();

            var resultBeforeChange = await this.usersRepository.All()
                .Where(u => u.Id == userIdToLock)
                .FirstOrDefaultAsync();

            Assert.Null(resultBeforeChange.LockoutEnd);

            await this.usersService.LockUserAsync(userIdToLock);

            var resultAfterChange = await this.usersRepository.All()
                .Where(u => u.Id == userIdToLock)
                .FirstOrDefaultAsync();

            Assert.NotNull(resultAfterChange.LockoutEnd);
            Assert.True(DateTime.UtcNow < resultAfterChange.LockoutEnd);
        }

        [Fact]
        public async Task TestLockUserAsync_WithIncorrectUserId_ShouldThrowNullRefEx()
        {
            var userIdToLock = "userId1";
            var incorrectUserIdToLock = "incorrectUserId1";

            var users = new List<TrainConnectedUser>()
            {
                new TrainConnectedUser
                {
                    Id = userIdToLock,
                    UserName = "TestUserName1",
                    LockoutEnd = DateTime.Now.AddDays(5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName2",
                    LockoutEnd = DateTime.Now.AddDays(-5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName3",
                    LockoutEnd = DateTime.Now,
                },
            };

            foreach (var user in users)
            {
                await this.usersRepository.AddAsync(user);
            }

            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.usersService.UnlockUserAsync(incorrectUserIdToLock));
        }

        [Fact]
        public async Task TestGetUserDetailsAsync_WithTestData_ShouldReturnUserDetails()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";

            var users = new List<TrainConnectedUser>()
            {
                new TrainConnectedUser
                {
                    Id = testUserId,
                    UserName = testUserName,
                    LockoutEnd = DateTime.Now.AddDays(5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName2",
                    LockoutEnd = DateTime.Now.AddDays(-5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName3",
                    LockoutEnd = DateTime.Now,
                },
            };

            foreach (var user in users)
            {
                await this.usersRepository.AddAsync(user);
            }

            await this.usersRepository.SaveChangesAsync();

            var expectedResult = await this.usersRepository.All()
                .To<UserDetailsViewModel>()
                .FirstOrDefaultAsync(u => u.Id == testUserId);

            var actualResult = await this.usersService.GetUserDetailsAsync(testUserId);

            Assert.Equal(expectedResult.Id, actualResult.Id);
            Assert.Equal(expectedResult.UserName, actualResult.UserName);
        }

        [Fact]
        public async Task TestGetUserDetailsAsync_WithIncorrectUser_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var incorrectUserId = "incorrectUserId";

            var users = new List<TrainConnectedUser>()
            {
                new TrainConnectedUser
                {
                    Id = testUserId,
                    UserName = testUserName,
                    LockoutEnd = DateTime.Now.AddDays(5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName2",
                    LockoutEnd = DateTime.Now.AddDays(-5),
                },
                new TrainConnectedUser
                {
                    UserName = "TestUserName3",
                    LockoutEnd = DateTime.Now,
                },
            };

            foreach (var user in users)
            {
                await this.usersRepository.AddAsync(user);
            }

            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.usersService.GetUserDetailsAsync(incorrectUserId));
        }

        [Fact]
        public async Task TestAddRoleAsync_WithIncorrectUser_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var incorectUserId = "incorectUserId";
            var testRoleName = "testRoleName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.usersService.AddRoleAsync(testRoleName, incorectUserId));
        }

        [Fact]
        public async Task TestAddRoleAsync_WithIncorrectRole_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testIncorrectRoleName = "testIncorrectRoleName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.usersService.AddRoleAsync(testIncorrectRoleName, testUserId));
        }

        [Fact]
        public async Task TestRemoveRoleAsync_WithIncorrectUser_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var incorectUserId = "incorectUserId";
            var testRoleName = "testRoleName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.usersService.RemoveRoleAsync(testRoleName, incorectUserId));
        }

        [Fact]
        public async Task TestRemoveRoleAsync_WithIncorrectRole_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId";
            var testUserName = "testUserName";
            var testIncorrectRoleName = "testIncorrectRoleName";

            var user = new TrainConnectedUser()
            {
                Id = testUserId,
                UserName = testUserName,
            };

            await this.usersRepository.AddAsync(user);
            await this.usersRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.usersService.RemoveRoleAsync(testIncorrectRoleName, testUserId));
        }
    }
}
