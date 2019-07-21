namespace TrainConnected.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;

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
    }
}
