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
        private readonly UserManager<TrainConnectedUser> userManager;

        public UsersService(IRepository<TrainConnectedUser> usersRepository, IRepository<IdentityUserRole<string>> usersRolesRepository, IRepository<IdentityRole<string>> rolesRepository, RoleManager<ApplicationRole> roleManager, UserManager<TrainConnectedUser> userManager)
        {
            this.usersRepository = usersRepository;
            this.usersRolesRepository = usersRolesRepository;
            this.rolesRepository = rolesRepository;
            this.roleManager = roleManager;
            this.userManager = userManager;
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
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
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
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
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
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
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

        public async Task AddRoleAsync(string roleName, string id)
        {
            var user = await this.usersRepository.All()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
            }

            var roleToAdd = await this.roleManager.FindByNameAsync(roleName);

            if (roleToAdd == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceRoleName, roleName));
            }

            await this.userManager.AddToRoleAsync(user, roleToAdd.Name);
        }

        public async Task RemoveRoleAsync(string roleName, string id)
        {
            var user = await this.usersRepository.All()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceUserId, id));
            }

            var roleToRemove = await this.roleManager.FindByNameAsync(roleName);

            if (roleToRemove == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.User.NullReferenceRoleName, roleName));
            }

            await this.userManager.RemoveFromRoleAsync(user, roleToRemove.Name);
        }
    }
}
