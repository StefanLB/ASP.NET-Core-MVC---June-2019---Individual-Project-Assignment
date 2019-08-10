namespace TrainConnected.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using TrainConnected.Common;
    using TrainConnected.Data.Models;

    internal class AdminSeeder : ISeeder
    {
        public async Task SeedAsync(TrainConnectedDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<TrainConnectedUser>>();

            var adminUser = new TrainConnectedUser()
            {
                UserName = GlobalConstants.AdministratorUserName,
                FirstName = GlobalConstants.AdministratorFirstName,
                LastName = GlobalConstants.AdministratorLastName,
                Email = GlobalConstants.AdministratorEmail,
                PhoneNumber = GlobalConstants.AdministratorPhoneNumber,
            };

            await SeedAdminAsync(userManager, adminUser);
        }

        private static async Task SeedAdminAsync(UserManager<TrainConnectedUser> userManager, TrainConnectedUser adminUser)
        {
            var result = await userManager.CreateAsync(adminUser, GlobalConstants.AdministratorPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, GlobalConstants.AdministratorRoleName);
            }
        }
    }
}
