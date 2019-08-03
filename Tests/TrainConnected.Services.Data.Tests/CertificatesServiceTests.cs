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
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.Certificates;
    using Xunit;

    public class CertificatesServiceTests
    {
        private readonly IRepository<Certificate> certificatesRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<WorkoutActivity> workoutActivityRepository;
        private readonly CertificatesService certificatesService;

        public CertificatesServiceTests()
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

            this.certificatesRepository = new EfRepository<Certificate>(dbContext);
            this.usersRepository = new EfRepository<TrainConnectedUser>(dbContext);
            this.workoutActivityRepository = new EfRepository<WorkoutActivity>(dbContext);

            this.certificatesService = new CertificatesService(this.certificatesRepository, this.usersRepository, this.workoutActivityRepository);
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllUserCertificates()
        {
            var testUserId = "testUserId1";
            var testDifferentUserId = "testUserId2";

            var certificates = new List<Certificate>()
            {
                new Certificate
                {
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId1",
                    Description = "TestDescriptionForTesting1",
                    IssuedBy = "TestOrganization1",
                },
                new Certificate
                {
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId2",
                    Description = "TestDescriptionForTesting2",
                    IssuedBy = "TestOrganization2",
                },
                new Certificate
                {
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId3",
                    Description = "TestDescriptionForTesting3",
                    IssuedBy = "TestOrganization3",
                },
                new Certificate
                {
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId4",
                    Description = "TestDescriptionForTesting4",
                    IssuedBy = "TestOrganization4",
                },
            };

            foreach (var certificate in certificates)
            {
                await this.certificatesRepository.AddAsync(certificate);
            }

            await this.certificatesRepository.SaveChangesAsync();

            var expectedResult = await this.certificatesRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .To<CertificatesAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.certificatesService.GetAllAsync(testUserId);

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(c =>
                    c.Id == result.Id
                    && c.IssuedBy == result.IssuedBy),
                    "CertificatesService GetAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllAsync_WithNoData_ShouldReturnNoUserCertificates()
        {
            var testUserId = "testUserId1";

            var actualResult = await this.certificatesService.GetAllAsync(testUserId);

            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithTestData_ShouldReturnCertificateDetails()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testDifferentUserId = "testUserId2";

            var certificates = new List<Certificate>()
            {
                new Certificate
                {
                    Id = testCertificateId,
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId1",
                    Description = "TestDescriptionForTesting1",
                    IssuedBy = testIssuedBy,
                },
                new Certificate
                {
                    Id = "certificateId2",
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId2",
                    Description = "TestDescriptionForTesting2",
                    IssuedBy = "TestOrganization2",
                },
                new Certificate
                {
                    Id = "certificateId3",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId3",
                    Description = "TestDescriptionForTesting3",
                    IssuedBy = "TestOrganization3",
                },
                new Certificate
                {
                    Id = "certificateId4",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId4",
                    Description = "TestDescriptionForTesting4",
                    IssuedBy = "TestOrganization4",
                },
            };

            foreach (var certificate in certificates)
            {
                await this.certificatesRepository.AddAsync(certificate);
            }

            await this.certificatesRepository.SaveChangesAsync();

            var result = await this.certificatesRepository.All()
                .Where(b => b.Id == testCertificateId)
                .FirstOrDefaultAsync();

#pragma warning disable CS4014
            this.certificatesService.GetDetailsAsync(testCertificateId, testUserId);
#pragma warning restore CS4014

            Assert.NotNull(result);
            Assert.True(testIssuedBy == result.IssuedBy, "CertificatesService GetDetailsAsync() does not work properly!");
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithIncorrectData_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var incorrectTestCertificateId = "incorrectTestCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testDifferentUserId = "testUserId2";

            var certificates = new List<Certificate>()
            {
                new Certificate
                {
                    Id = testCertificateId,
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId1",
                    Description = "TestDescriptionForTesting1",
                    IssuedBy = testIssuedBy,
                },
                new Certificate
                {
                    Id = "certificateId2",
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId2",
                    Description = "TestDescriptionForTesting2",
                    IssuedBy = "TestOrganization2",
                },
                new Certificate
                {
                    Id = "certificateId3",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId3",
                    Description = "TestDescriptionForTesting3",
                    IssuedBy = "TestOrganization3",
                },
                new Certificate
                {
                    Id = "certificateId4",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId4",
                    Description = "TestDescriptionForTesting4",
                    IssuedBy = "TestOrganization4",
                },
            };

            foreach (var certificate in certificates)
            {
                await this.certificatesRepository.AddAsync(certificate);
            }

            await this.certificatesRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.certificatesService.GetDetailsAsync(incorrectTestCertificateId, testUserId));
        }

        [Fact]
        public async Task TestCreateAsync_WithTestData_ShouldCreateCertificate()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            var certificateCreateInputModel = AutoMapper.Mapper.Map<CertificateCreateInputModel>(certificate);
            certificateCreateInputModel.Activity = testActivityName;

            var actualResult = await this.certificatesService.CreateAsync(certificateCreateInputModel, testUserId);

            var expectedResult = await this.certificatesRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .To<CertificateDetailsViewModel>()
                .FirstOrDefaultAsync();

            Assert.Equal(actualResult.ActivityName, expectedResult.ActivityName);
            Assert.Equal(actualResult.Description, expectedResult.Description);
            Assert.Equal(actualResult.IssuedBy, expectedResult.IssuedBy);
        }

        [Fact]
        public async Task TestCreateAsync_WithoutWorkoutActivity_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            var certificateCreateInputModel = AutoMapper.Mapper.Map<CertificateCreateInputModel>(certificate);
            certificateCreateInputModel.Activity = testActivityName;

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.certificatesService.CreateAsync(certificateCreateInputModel, testUserId));
        }

        [Fact]
        public async Task TestGetEditDetailsAsync_WithTestData_ShouldReturnCertificateDetails()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testDifferentUserId = "testUserId2";

            var certificates = new List<Certificate>()
            {
                new Certificate
                {
                    Id = testCertificateId,
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId1",
                    Description = "TestDescriptionForTesting1",
                    IssuedBy = testIssuedBy,
                },
                new Certificate
                {
                    Id = "certificateId2",
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId2",
                    Description = "TestDescriptionForTesting2",
                    IssuedBy = "TestOrganization2",
                },
                new Certificate
                {
                    Id = "certificateId3",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId3",
                    Description = "TestDescriptionForTesting3",
                    IssuedBy = "TestOrganization3",
                },
                new Certificate
                {
                    Id = "certificateId4",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId4",
                    Description = "TestDescriptionForTesting4",
                    IssuedBy = "TestOrganization4",
                },
            };

            foreach (var certificate in certificates)
            {
                await this.certificatesRepository.AddAsync(certificate);
            }

            await this.certificatesRepository.SaveChangesAsync();

            var result = await this.certificatesRepository.All()
                .Where(b => b.Id == testCertificateId)
                .FirstOrDefaultAsync();

            #pragma warning disable CS4014
            this.certificatesService.GetEditDetailsAsync(testCertificateId, testUserId);
            #pragma warning restore CS4014

            Assert.NotNull(result);
            Assert.True(testIssuedBy == result.IssuedBy, "CertificatesService GetEditDetailsAsync() does not work properly!");
        }

        [Fact]
        public async Task TestGetEditDetailsAsync_WithIncorrectData_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var incorrectTestCertificateId = "incorrectTestCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testDifferentUserId = "testUserId2";

            var certificates = new List<Certificate>()
            {
                new Certificate
                {
                    Id = testCertificateId,
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId1",
                    Description = "TestDescriptionForTesting1",
                    IssuedBy = testIssuedBy,
                },
                new Certificate
                {
                    Id = "certificateId2",
                    TrainConnectedUserId = testUserId,
                    ActivityId = "testId2",
                    Description = "TestDescriptionForTesting2",
                    IssuedBy = "TestOrganization2",
                },
                new Certificate
                {
                    Id = "certificateId3",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId3",
                    Description = "TestDescriptionForTesting3",
                    IssuedBy = "TestOrganization3",
                },
                new Certificate
                {
                    Id = "certificateId4",
                    TrainConnectedUserId = testDifferentUserId,
                    ActivityId = "testId4",
                    Description = "TestDescriptionForTesting4",
                    IssuedBy = "TestOrganization4",
                },
            };

            foreach (var certificate in certificates)
            {
                await this.certificatesRepository.AddAsync(certificate);
            }

            await this.certificatesRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.certificatesService.GetEditDetailsAsync(incorrectTestCertificateId, testUserId));
        }

        [Fact]
        public async Task TestUpdateAsync_WithTestData_ShouldUpdateCertificate()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";
            var updatedIssuedById = "TestOrganization2";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            var certificateEditInputModel = AutoMapper.Mapper.Map<CertificateEditInputModel>(certificate);
            certificateEditInputModel.IssuedBy = updatedIssuedById;

            await this.certificatesService.UpdateAsync(certificateEditInputModel, testUserId);

            var actualResult = await this.certificatesRepository.All()
                .Where(u => u.TrainConnectedUserId == testUserId)
                .FirstOrDefaultAsync();

            Assert.Equal(updatedIssuedById, actualResult.IssuedBy);
        }

        [Fact]
        public async Task TestUpdateAsync_WithIncorrectCertificate_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";
            var updatedIssuedById = "TestOrganization2";
            var incorrectCertificateId = "incorrectCertificateId";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            var certificateEditInputModel = AutoMapper.Mapper.Map<CertificateEditInputModel>(certificate);
            certificateEditInputModel.IssuedBy = updatedIssuedById;
            certificateEditInputModel.Id = incorrectCertificateId;

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.certificatesService.UpdateAsync(certificateEditInputModel, testUserId));
        }

        [Fact]
        public async Task TestUpdateAsync_WithIncorrectUser_ShouldThrowArgEx()
        {
            var testUserId = "testUserId1";
            var incorrectUserId = "incorrectUserId";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";
            var updatedIssuedById = "TestOrganization2";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            var incorrectTestUser = new TrainConnectedUser()
            {
                Id = incorrectUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.AddAsync(incorrectTestUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            var certificateEditInputModel = AutoMapper.Mapper.Map<CertificateEditInputModel>(certificate);
            certificateEditInputModel.IssuedBy = updatedIssuedById;

            await Assert.ThrowsAsync<ArgumentException>(async () => await this.certificatesService.UpdateAsync(certificateEditInputModel, incorrectUserId));
        }

        [Fact]
        public async Task TestUpdateAsync_WithMissingActivity_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";
            var incorrectActivityName = "incorrectActivityName";
            var updatedIssuedById = "TestOrganization2";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            var certificateEditInputModel = AutoMapper.Mapper.Map<CertificateEditInputModel>(certificate);
            certificateEditInputModel.IssuedBy = updatedIssuedById;
            certificateEditInputModel.ActivityName = incorrectActivityName;

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.certificatesService.UpdateAsync(certificateEditInputModel, testUserId));
        }

        [Fact]
        public async Task TestDeleteAsync_WithTestData_ShouldDeleteCertificate()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            Assert.NotEmpty(await this.certificatesRepository.All().ToArrayAsync());

            await this.certificatesService.DeleteAsync(testCertificateId, testUserId);

            Assert.Empty(await this.certificatesRepository.All().ToArrayAsync());
        }

        [Fact]
        public async Task TestDeleteAsync_WithMissingCertificate_ShouldThrowNullRefEx()
        {
            var testUserId = "testUserId1";
            var testCertificateId = "testCertificateId";
            var incorrectCertificateId = "incorrectCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            Assert.NotEmpty(await this.certificatesRepository.All().ToArrayAsync());

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.certificatesService.DeleteAsync(incorrectCertificateId, testUserId));
        }

        [Fact]
        public async Task TestDeleteAsync_WithIncorrectUserId_ShouldThrowArgEx()
        {
            var testUserId = "testUserId1";
            var incorrectUserId = "incorrectUserId";
            var testCertificateId = "testCertificateId";
            var testIssuedBy = "TestOrganization1";
            var testActivityName = "activityName1";

            var workoutActivity = new WorkoutActivity()
            {
                Id = "testId1",
                Name = testActivityName,
                Icon = "testIconUrl1",
                Description = "TestDescription1",
            };

            await this.workoutActivityRepository.AddAsync(workoutActivity);
            await this.workoutActivityRepository.SaveChangesAsync();

            var testUser = new TrainConnectedUser()
            {
                Id = testUserId,
            };

            var incorrectUser = new TrainConnectedUser()
            {
                Id = incorrectUserId,
            };

            await this.usersRepository.AddAsync(testUser);
            await this.usersRepository.AddAsync(incorrectUser);
            await this.usersRepository.SaveChangesAsync();

            var certificate = new Certificate()
            {
                Id = testCertificateId,
                TrainConnectedUserId = testUserId,
                ActivityId = "testId1",
                Description = "TestDescriptionForTesting1",
                IssuedBy = testIssuedBy,
                Activity = workoutActivity,
                IssuedOn = DateTime.Now.AddDays(-5),
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            Assert.NotEmpty(await this.certificatesRepository.All().ToArrayAsync());

            await Assert.ThrowsAsync<ArgumentException>(async () => await this.certificatesService.DeleteAsync(testCertificateId, incorrectUserId));
        }
    }
}
