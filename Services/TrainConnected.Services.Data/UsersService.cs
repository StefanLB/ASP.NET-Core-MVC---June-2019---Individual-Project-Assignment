namespace TrainConnected.Services.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
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
        private readonly IRepository<IdentityRole<string>> rolesRepository;
        private readonly RoleManager<ApplicationRole> roleManager;

        public UsersService(IRepository<TrainConnectedUser> usersRepository, IRepository<IdentityUserRole<string>> usersRolesRepository, IRepository<IdentityRole<string>> rolesRepository, RoleManager<ApplicationRole> roleManager)
        {
            this.usersRepository = usersRepository;
            this.usersRolesRepository = usersRolesRepository;
            this.rolesRepository = rolesRepository;
            this.roleManager = roleManager;
        }

        public async Task<IEnumerable<UsersAllViewModel>> GetAllAsync(string adminId)
        {
            var users = await this.usersRepository.All()
                .Where(x => x.Id != adminId)
                .To<UsersAllViewModel>()
                .OrderBy(x => x.UserName)
                .ToArrayAsync();

            return users;
        }

        public async Task UnlockUserAsync(string id)
        {
            var user = await this.usersRepository.All()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            user.LockoutEnd = null;

            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task LockUserAsync(string id)
        {
            var user = await this.usersRepository.All()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task<UserDetailsViewModel> GetUserDetailsAsync(string id)
        {
            var user = await this.usersRepository.All()
                .Where(x => x.Id == id)
                .To<UserDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var userRolesIds = await this.usersRolesRepository.All()
                .Where(u => u.UserId == id)
                .Select(r => r.RoleId)
                .ToArrayAsync();

            var userRolesNames = new List<string>();

            foreach (var roleId in userRolesIds)
            {
                var role = await this.roleManager.FindByIdAsync(roleId);
                var roleNameToAdd = role.Name;

                userRolesNames.Add(roleNameToAdd);
            }

            user.RolesNames = userRolesNames;

            return user;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            var rolesNames = await this.roleManager.Roles
                .Select(n => n.Name)
                .ToArrayAsync();

            return rolesNames;
        }
    }
}
