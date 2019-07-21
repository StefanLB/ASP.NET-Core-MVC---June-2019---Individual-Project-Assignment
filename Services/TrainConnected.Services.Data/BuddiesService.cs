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

        public BuddiesService(IRepository<TrainConnectedUser> usersRepository)
        {
            this.usersRepository = usersRepository;
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

            if (buddyToAdd == null)
            {
                throw new InvalidOperationException();
            }

            user.Buddies.Add(buddyToAdd);

            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<BuddiesAllViewModel>> FindAllAsync(string userId)
        {
            var buddiesIds = await this.usersRepository.All()
                .Where(x => x.Id == userId)
                .Select(b => b.Buddies)
                .To<BuddiesAllViewModel>()
                .Select(i => i.Id)
                .ToArrayAsync();

            var nonBuddies = await this.usersRepository.All()
                .Where(x => !buddiesIds.Contains(x.Id))
                .To<BuddiesAllViewModel>()
                .ToArrayAsync();

            return nonBuddies;
        }

        public async Task<IEnumerable<BuddiesAllViewModel>> GetAllAsync(string userId)
        {
            var buddies = await this.usersRepository.All()
                .Where(x => x.Id == userId)
                .Select(b => b.Buddies)
                .To<BuddiesAllViewModel>()
                .ToArrayAsync();

            return buddies;
        }

        public async Task<BuddyDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var buddy = await this.usersRepository.All()
                .To<BuddyDetailsViewModel>()
                .FirstOrDefaultAsync(x => x.Id == id);

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

            buddy.AddedOn = user.Buddies.Where(x => x.Id == buddy.Id).First().CreatedOn;

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

            user.Buddies.Remove(buddyToRemove);

            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
        }
    }
}
