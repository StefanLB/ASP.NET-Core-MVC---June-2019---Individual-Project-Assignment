namespace TrainConnected.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
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

            if (user.Balance >= withdrawalCreateInputModel.Amount && withdrawalCreateInputModel.Amount > 0)
            {
                var withdrawal = new Withdrawal
                {
                    Amount = withdrawalCreateInputModel.Amount,
                    TrainConnectedUserId = user.Id,
                    TrainConnectedUser = user,
                };

                await this.withdrawalsRepository.AddAsync(withdrawal);
                await this.withdrawalsRepository.SaveChangesAsync();

                user.Balance -= withdrawalCreateInputModel.Amount;

                this.usersRepository.Update(user);
                await this.usersRepository.SaveChangesAsync();
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
    }
}
