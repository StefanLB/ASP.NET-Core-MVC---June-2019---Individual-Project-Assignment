namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrainConnected.Web.InputModels.Withdrawals;
    using TrainConnected.Web.ViewModels.Withdrawals;

    public interface IWithdrawalsService
    {
        Task<IEnumerable<WithdrawalsProcessingViewModel>> GetAllAdminAsync();

        Task<IEnumerable<WithdrawalsAllViewModel>> GetAllAsync(string userId);

        Task CreateAsync(WithdrawalCreateInputModel withdrawalCreateInputModel, string userId);

        Task<WithdrawalsProcessingViewModel> GetForProcessingAsync(string id);

        Task ProcessAsync(WithdrawalProcessInputModel withdrawalProcessInputModel, string processedByUserId);

        Task<decimal> GetUserBalanceAsync(string userId);

        Task<decimal> GetUserPendingWithdrawalsBalance(string userId);
    }
}
