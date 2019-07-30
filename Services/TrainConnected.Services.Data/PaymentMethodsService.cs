namespace TrainConnected.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.PaymentMethods;
    using TrainConnected.Web.ViewModels.PaymentMethods;

    public class PaymentMethodsService : IPaymentMethodsService
    {
        private readonly IRepository<PaymentMethod> paymentMethodsRepository;

        public PaymentMethodsService(IRepository<PaymentMethod> paymentMethodsRepository)
        {
            this.paymentMethodsRepository = paymentMethodsRepository;
        }

        public async Task<IEnumerable<PaymentMethodsAllViewModel>> GetAllAsync()
        {
            var paymentMethods = await this.paymentMethodsRepository.All()
                .To<PaymentMethodsAllViewModel>()
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return paymentMethods;
        }

        public async Task<PaymentMethodDetailsViewModel> GetDetailsAsync(string id)
        {
            var paymentMethod = await this.paymentMethodsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (paymentMethod == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.PaymentMethod.NullReferencePaymentMethodId, id));
            }

            var paymentMethodDetailsViewModel = AutoMapper.Mapper.Map<PaymentMethodDetailsViewModel>(paymentMethod);
            return paymentMethodDetailsViewModel;
        }

        public async Task<PaymentMethodDetailsViewModel> CreateAsync(PaymentMethodCreateInputModel paymentMethodCreateInputModel)
        {
            var checkPaymentMethodExists = this.paymentMethodsRepository.All()
                .FirstOrDefault(x => x.Name == paymentMethodCreateInputModel.Name);

            if (checkPaymentMethodExists != null)
            {
                throw new InvalidOperationException(string.Format(ServiceConstants.PaymentMethod.PaymentMethodNameAlreadyExists, paymentMethodCreateInputModel.Name));
            }

            var paymentMethod = new PaymentMethod
            {
                Name = paymentMethodCreateInputModel.Name,
                PaymentInAdvance = paymentMethodCreateInputModel.PaymentInAdvance,
            };

            await this.paymentMethodsRepository.AddAsync(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();

            var paymentMethodDetailsViewModel = AutoMapper.Mapper.Map<PaymentMethodDetailsViewModel>(paymentMethod);
            return paymentMethodDetailsViewModel;
        }

        public async Task DeleteAsync(string id)
        {
            var paymentMethod = await this.paymentMethodsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (paymentMethod == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.PaymentMethod.NullReferencePaymentMethodId, id));
            }

            paymentMethod.IsDeleted = true;
            this.paymentMethodsRepository.Update(paymentMethod);
            await this.paymentMethodsRepository.SaveChangesAsync();
        }
    }
}
