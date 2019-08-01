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
    using TrainConnected.Web.InputModels.PaymentMethods;
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels;
    using TrainConnected.Web.ViewModels.PaymentMethods;
    using Xunit;

    public class PaymentMethodsServiceTests
    {
        private readonly IRepository<PaymentMethod> paymentMethodsRepository;
        private readonly PaymentMethodsService paymentMethodsService;

        public PaymentMethodsServiceTests()
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

            this.paymentMethodsRepository = new EfRepository<PaymentMethod>(dbContext);
            this.paymentMethodsService = new PaymentMethodsService(this.paymentMethodsRepository);
        }

        [Fact]
        public async Task TestGetAllAsync_WithTestData_ShouldReturnAllPaymentMethods()
        {
            var paymentMethods = new List<PaymentMethod>()
            {
                new PaymentMethod
                {
                    Name = "Cash",
                    PaymentInAdvance = false,
                },
                new PaymentMethod
                {
                    Name = "Epay",
                    PaymentInAdvance = true,
                },
            };

            foreach (var pm in paymentMethods)
            {
                await this.paymentMethodsRepository.AddAsync(pm);
            }

            await this.paymentMethodsRepository.SaveChangesAsync();

            var expectedResult = await this.paymentMethodsRepository.All()
                .To<PaymentMethodsAllViewModel>()
                .ToArrayAsync();

            var actualResult = await this.paymentMethodsService.GetAllAsync();

            Assert.Equal(expectedResult.Count(), actualResult.Count());

            foreach (var result in actualResult)
            {
                Assert.True(
                    expectedResult.Any(pm =>
                    pm.Name == result.Name
                    && pm.PaymentInAdvance == result.PaymentInAdvance),
                    "PaymentMethodService GetAllAsync() does not work properly!");
            }
        }

        [Fact]
        public async Task TestGetAllAsync_WithNoData_ShouldReturnEmptyCollection()
        {
            IEnumerable<PaymentMethodsAllViewModel> expectedResult = new List<PaymentMethodsAllViewModel>();

            var actualResult = await this.paymentMethodsService.GetAllAsync();

            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithInCorrectData_ShouldThrowNullRefEx()
        {
            var pmIdToRetrieve = "GetThisPm";
            var pmNameToCheck = "Cash";

            var paymentMethods = new List<PaymentMethod>()
            {
                new PaymentMethod
                {
                    Id = pmIdToRetrieve,
                    Name = pmNameToCheck,
                    PaymentInAdvance = false,
                },
                new PaymentMethod
                {
                    Id = "DontGetThisPm",
                    Name = "Epay",
                    PaymentInAdvance = true,
                },
            };

            foreach (var pm in paymentMethods)
            {
                await this.paymentMethodsRepository.AddAsync(pm);
            }

            await this.paymentMethodsRepository.SaveChangesAsync();

            var expectedResult = await this.paymentMethodsRepository.All()
                .Where(x => x.Id == pmIdToRetrieve)
                .To<PaymentMethodDetailsViewModel>()
                .FirstOrDefaultAsync();

            var actualResult = await this.paymentMethodsService.GetDetailsAsync(pmIdToRetrieve);

            Assert.Equal(expectedResult.Id, actualResult.Id);
            Assert.Equal(expectedResult.Name, actualResult.Name);
        }

        [Fact]
        public async Task TestGetDetailsAsync_WithCorrectData_ShouldReturnDetails()
        {
            var incorrectId = "incorrectId";

            var paymentMethods = new List<PaymentMethod>()
            {
                new PaymentMethod
                {
                    Id = "GetThisPm",
                    Name = "Cash",
                    PaymentInAdvance = false,
                },
                new PaymentMethod
                {
                    Id = "DontGetThisPm",
                    Name = "Epay",
                    PaymentInAdvance = true,
                },
            };

            foreach (var pm in paymentMethods)
            {
                await this.paymentMethodsRepository.AddAsync(pm);
            }

            await this.paymentMethodsRepository.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.paymentMethodsService.GetDetailsAsync(incorrectId));
        }

        [Fact]
        public async Task TestCreateAsync_WithCorrectData_ShouldCreatePM()
        {
            var paymentMethodName = "GetThisPm";
            var paymentMethodPIA = true;

            await this.paymentMethodsService.CreateAsync(new PaymentMethodCreateInputModel
            {
                Name = paymentMethodName,
                PaymentInAdvance = paymentMethodPIA,
            });

            var expectedResult = await this.paymentMethodsRepository.All()
                .Where(n => n.Name == paymentMethodName)
                .Where(pia => pia.PaymentInAdvance == paymentMethodPIA)
                .To<PaymentMethodDetailsViewModel>()
                .FirstOrDefaultAsync();

            Assert.NotNull(expectedResult);
        }

        [Fact]
        public async Task TestCreateAsync_WithDuplicateData_ShouldThrowInvOpEx()
        {
            var paymentMethodDuplicateName = "GetThisPm";
            var paymentMethodPIA = true;

            var initialEntry = new PaymentMethodCreateInputModel
            {
                Name = paymentMethodDuplicateName,
                PaymentInAdvance = paymentMethodPIA,
            };

            var duplicateEntry = new PaymentMethodCreateInputModel
            {
                Name = paymentMethodDuplicateName,
                PaymentInAdvance = paymentMethodPIA,
            };

            await this.paymentMethodsService.CreateAsync(initialEntry);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.paymentMethodsService.CreateAsync(duplicateEntry));
        }

        [Fact]
        public async Task TestDeleteAsync_WithCorrectData_ShouldDeletePm()
        {
            var paymentMethodName = "GetThisPm";
            var paymentMethodPIA = true;

            var paymentMethod = new PaymentMethod
            {
                Name = paymentMethodName,
                PaymentInAdvance = paymentMethodPIA,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            var initialArray = await this.paymentMethodsRepository.All().ToArrayAsync();
            var initialCount = initialArray.Count();

            var paymentMethodToDeleteId = initialArray.Select(x => x.Id).FirstOrDefault();

            await this.paymentMethodsService.DeleteAsync(paymentMethodToDeleteId);

            var finalArray = await this.paymentMethodsRepository.All().ToArrayAsync();
            var finalCount = finalArray.Count();

            Assert.True(initialCount > finalCount);
        }

        [Fact]
        public async Task TestDeleteAsync_WithInCorrectData_ShouldThrowNullRefEx()
        {
            var paymentMethodName = "GetThisPm";
            var paymentMethodPIA = true;

            var paymentMethod = new PaymentMethod
            {
                Name = paymentMethodName,
                PaymentInAdvance = paymentMethodPIA,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            var initialArray = await this.paymentMethodsRepository.All().ToArrayAsync();
            var initialCount = initialArray.Count();

            var incorrectPaymentMethodToDeleteId = "GetThisIncorrectPm";

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.paymentMethodsService.DeleteAsync(incorrectPaymentMethodToDeleteId));
        }
    }
}
