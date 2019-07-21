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
    using TrainConnected.Web.ViewModels.Buddies;

    public class BuddiesService : IBuddiesService
    {
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<TrainConnectedUsersBuddies> usersBuddiesRepository;

        public BuddiesService(IRepository<TrainConnectedUser> usersRepository, IRepository<TrainConnectedUsersBuddies> usersBuddiesRepository)
        {
            this.usersRepository = usersRepository;
            this.usersBuddiesRepository = usersBuddiesRepository;
        }

        public async Task AddAsync(string id, string userId)
        {
            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var buddyToAdd = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (buddyToAdd == null || buddyToAdd.Id == user.Id)
            {
                throw new InvalidOperationException();
            }

            await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies
            {
                TrainConnectedUserId = user.Id,
                TrainConnectedBuddyId = buddyToAdd.Id,
                AddedOn = DateTime.UtcNow,
            });

            await this.usersBuddiesRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<BuddiesAllViewModel>> FindAllAsync(string userId)
        {
            var buddiesIds = await this.usersBuddiesRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .Select(b => b.TrainConnectedBuddyId)
                .ToArrayAsync();

            var nonBuddies = await this.usersRepository.All()
                .Where(x => !buddiesIds.Contains(x.Id) && x.Id != userId)
                .To<BuddiesAllViewModel>()
                .OrderBy(x => x.UserName)
                .ToArrayAsync();

            return nonBuddies;
        }

        public async Task<IEnumerable<BuddiesAllViewModel>> GetAllAsync(string userId)
        {

            var buddies = await this.usersBuddiesRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .Select(b => b.TrainConnectedBuddy)
                .To<BuddiesAllViewModel>()
                .OrderBy(x => x.UserName)
                .ToArrayAsync();

            return buddies;
        }

        public async Task<BuddyDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var buddy = await this.usersRepository.All()
                .Where(x => x.Id == id)
                .To<BuddyDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (buddy == null)
            {
                throw new InvalidOperationException();
            }

            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            buddy.AddedOn = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == userId)
                .Where(b => b.TrainConnectedBuddyId == id)
                .Select(x => x.AddedOn)
                .FirstOrDefaultAsync();

            return buddy;
        }

        public async Task RemoveAsync(string id, string userId)
        {
            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var buddyToRemove = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (buddyToRemove == null)
            {
                throw new InvalidOperationException();
            }

            var userBuddyConnection = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == userId)
                .Where(b => b.TrainConnectedBuddyId == id)
                .FirstOrDefaultAsync();

            this.usersBuddiesRepository.Delete(userBuddyConnection);
            await this.usersBuddiesRepository.SaveChangesAsync();
        }
    }
}
