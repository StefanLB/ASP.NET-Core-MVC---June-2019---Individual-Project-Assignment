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
    using TrainConnected.Web.ViewModels.Buddies;
    using TrainConnected.Web.ViewModels.Certificates;

    public class BuddiesService : IBuddiesService
    {
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<TrainConnectedUsersBuddies> usersBuddiesRepository;
        private readonly IRepository<Certificate> certificatesRepository;
        private readonly IRepository<Workout> workoutsRepository;

        public BuddiesService(IRepository<TrainConnectedUser> usersRepository, IRepository<TrainConnectedUsersBuddies> usersBuddiesRepository, IRepository<Certificate> certificatesRepository, IRepository<Workout> workoutsRepository)
        {
            this.usersRepository = usersRepository;
            this.usersBuddiesRepository = usersBuddiesRepository;
            this.certificatesRepository = certificatesRepository;
            this.workoutsRepository = workoutsRepository;
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

        public async Task<BuddyDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var buddy = await this.usersRepository.All()
                .Where(x => x.Id == id)
                .To<BuddyDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (buddy == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
            }

            var userBuddyConnection = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == userId)
                .Where(b => b.TrainConnectedBuddyId == id)
                .FirstOrDefaultAsync();

            if (userBuddyConnection == null)
            {
                throw new ArgumentException(string.Format(ServiceConstants.User.ArgumentUserBuddyMismatch, id, userId));
            }

            buddy.AddedOn = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == userId)
                .Where(b => b.TrainConnectedBuddyId == id)
                .Select(x => x.AddedOn)
                .FirstOrDefaultAsync();

            return buddy;
        }

        public async Task<CoachDetailsViewModel> GetCoachDetailsAsync(string coachUserName)
        {
            var coachDetailsViewModel = await this.usersRepository.All()
                .Where(x => x.UserName == coachUserName)
                .To<CoachDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (coachDetailsViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceCoachName, coachUserName));
            }

            coachDetailsViewModel.Certificates = await this.certificatesRepository.All()
                .Where(x => x.TrainConnectedUserId == coachDetailsViewModel.Id)
                .Where(e => (!e.ExpiresOn.HasValue) || (e.ExpiresOn > DateTime.UtcNow))
                .To<CertificatesAllViewModel>()
                .ToArrayAsync();

            coachDetailsViewModel.WorkoutsCoached = this.workoutsRepository.All()
                .Where(c => c.CoachId == coachDetailsViewModel.Id)
                .Count();

            return coachDetailsViewModel;
        }

        public async Task AddAsync(string id, string userId)
        {
            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, userId));
            }

            var buddyToAdd = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (buddyToAdd == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
            }

            var userBuddyConnection = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == userId)
                .Where(b => b.TrainConnectedBuddyId == id)
                .FirstOrDefaultAsync();

            if (userBuddyConnection != null || buddyToAdd.Id == user.Id)
            {
                throw new InvalidOperationException(string.Format(ServiceConstants.User.BefriendingCriteriaNotMet));
            }

            await this.usersBuddiesRepository.AddAsync(new TrainConnectedUsersBuddies
            {
                TrainConnectedUserId = user.Id,
                TrainConnectedBuddyId = buddyToAdd.Id,
                AddedOn = DateTime.UtcNow,
            });

            await this.usersBuddiesRepository.SaveChangesAsync();
        }

        public async Task RemoveAsync(string id, string userId)
        {
            var buddyToRemove = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (buddyToRemove == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
            }

            var userBuddyConnection = await this.usersBuddiesRepository.All()
                .Where(u => u.TrainConnectedUserId == userId)
                .Where(b => b.TrainConnectedBuddyId == id)
                .FirstOrDefaultAsync();

            if (userBuddyConnection == null)
            {
                throw new ArgumentException(string.Format(ServiceConstants.User.ArgumentUserBuddyMismatch, id, userId));
            }

            this.usersBuddiesRepository.Delete(userBuddyConnection);
            await this.usersBuddiesRepository.SaveChangesAsync();
        }
    }
}
