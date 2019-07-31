namespace TrainConnected.Services.Data.Tests
{
    using AutoMapper;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using TrainConnected.Data;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Data.Repositories;
    using TrainConnected.Services.Mapping;
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



    }
}
