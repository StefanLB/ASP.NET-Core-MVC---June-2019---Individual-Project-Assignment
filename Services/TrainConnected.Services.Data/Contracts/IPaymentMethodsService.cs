namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.InputModels.PaymentMethods;
    using TrainConnected.Web.ViewModels.PaymentMethods;

    public interface IPaymentMethodsService
    {
        Task<PaymentMethodDetailsViewModel> GetDetailsAsync(string id);

        Task<IEnumerable<PaymentMethodsAllViewModel>> GetAllAsync();

        Task<PaymentMethodDetailsViewModel> CreateAsync(PaymentMethodCreateInputModel paymentMethodCreateInputModel);

        Task DeleteAsync(string id);
    }
}
