namespace TrainConnected.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.Withdrawals;
    using TrainConnected.Web.ViewModels.Withdrawals;

    public class WithdrawalsService : IWithdrawalsService
    {
        private readonly IRepository<Withdrawal> withdrawalsRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;

        public WithdrawalsService(IRepository<Withdrawal> withdrawalsRepository, IRepository<TrainConnectedUser> usersRepository)
        {
            this.withdrawalsRepository = withdrawalsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task CreateAsync(WithdrawalCreateInputModel withdrawalCreateInputModel, string userId)
        {
            var user = this.usersRepository.All()
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException();
            }

            var pendingWithdrawals = await this.GetUserPendingWithdrawalsBalance(userId);
            var withdrawableAmount = user.Balance - pendingWithdrawals;

            if (withdrawableAmount >= withdrawalCreateInputModel.Amount && withdrawalCreateInputModel.Amount > 0)
            {
                var withdrawal = new Withdrawal
                {
                    Amount = withdrawalCreateInputModel.Amount,
                    AdditionalInstructions = withdrawalCreateInputModel.AdditionalInstructions,
                    TrainConnectedUserId = user.Id,
                    TrainConnectedUser = user,
                    Status = StatusCode.Initiated,
                    FinalizedOn = DateTime.MinValue,
                };

                await this.withdrawalsRepository.AddAsync(withdrawal);
                await this.withdrawalsRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WithdrawalsAllViewModel>> GetAllAsync(string userId)
        {
            var withdrawals = await this.withdrawalsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .To<WithdrawalsAllViewModel>()
                .OrderByDescending(x => x.CreatedOn)
                .ToArrayAsync();

            return withdrawals;
        }

        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException();
            }

            var balance = user.Balance;

            return balance;
        }

        public async Task<decimal> GetUserPendingWithdrawalsBalance(string userId)
        {
            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException();
            }

            var pendingUserWithdrawals = await this.withdrawalsRepository.All()
                .Where(u => u.TrainConnectedUserId == userId)
                .Where(s => s.Status == StatusCode.Initiated || s.Status == StatusCode.InProcess)
                .Select(a => a.Amount)
                .ToArrayAsync();

            var pendingUserWithdrawalsAmount = 0.00m;

            if (pendingUserWithdrawals.Count() > 0)
            {
                pendingUserWithdrawalsAmount = pendingUserWithdrawals.Sum();
            }

            return pendingUserWithdrawalsAmount;
        }
    }
}
