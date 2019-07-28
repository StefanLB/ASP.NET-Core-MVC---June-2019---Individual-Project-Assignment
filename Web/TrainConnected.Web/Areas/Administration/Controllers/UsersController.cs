namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;

    public class UsersController : AdministrationController
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var adminId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = await this.usersService.GetAllAsync(adminId);

            return this.View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            await this.usersService.UnlockUserAsync(id);

            return this.RedirectToAction(nameof(this.All));
        }

        [HttpGet]
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            await this.usersService.LockUserAsync(id);

            return this.RedirectToAction(nameof(this.All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userDetails = await this.usersService.GetUserDetailsAsync(id);
            this.ViewData["Roles"] = await this.usersService.GetAllRolesAsync();

            return this.View(userDetails);
        }

        [HttpGet]
        public async Task<IActionResult> AddRole(string roleName, string id)
        {
            if (roleName == null || id == null)
            {
                return this.NotFound();
            }

            await this.usersService.AddRoleAsync(roleName, id);

            return this.RedirectToAction(nameof(this.All));
        }

        [HttpGet]
        public async Task<IActionResult> RemoveRole(string roleName, string id)
        {
            if (roleName == null || id == null)
            {
                return this.NotFound();
            }

            await this.usersService.RemoveRoleAsync(roleName, id);

            return this.RedirectToAction(nameof(this.All));
        }
    }
}
