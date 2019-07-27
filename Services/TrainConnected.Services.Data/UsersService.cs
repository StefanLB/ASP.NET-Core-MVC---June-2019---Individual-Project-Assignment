namespace TrainConnected.Services.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.ViewModels.Users;

    public class UsersService : IUsersService
    {
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<IdentityUserRole<string>> usersRolesRepository;

        public UsersService(IRepository<TrainConnectedUser> usersRepository, IRepository<IdentityUserRole<string>> usersRolesRepository)
        {
            this.usersRepository = usersRepository;
            this.usersRolesRepository = usersRolesRepository;
        }

        public async Task<IEnumerable<UsersAllViewModel>> GetAllAsync()
        {
            var users = await this.usersRepository.All()
                .To<UsersAllViewModel>()
                .OrderBy(x => x.UserName)
                .ToArrayAsync();

            return users;
        }
    }
}
