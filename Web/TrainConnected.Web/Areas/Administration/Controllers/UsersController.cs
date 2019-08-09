namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.Helpers;
    using TrainConnected.Web.ViewModels.Users;

    public class UsersController : AdministrationController
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> All(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            this.ViewData["CurrentSort"] = sortOrder;
            this.ViewData["UserNameSortParm"] = sortOrder == "UserName" ? "userName_desc" : "UserName";
            this.ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "firstName_desc" : "FirstName";
            this.ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "lastName_desc" : "LastName";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            this.ViewData["CurrentFilter"] = searchString;

            var adminId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = await this.usersService.GetAllAsync(adminId);

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(b => b.UserName.ToLower().Contains(searchString.ToLower()) ||
                                             b.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                             b.LastName.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "UserName":
                    users = users.OrderBy(b => b.UserName);
                    break;
                case "userName_desc":
                    users = users.OrderByDescending(b => b.UserName);
                    break;
                case "FirstName":
                    users = users.OrderBy(b => b.FirstName);
                    break;
                case "firstName_desc":
                    users = users.OrderByDescending(b => b.FirstName);
                    break;
                case "LastName":
                    users = users.OrderBy(b => b.LastName);
                    break;
                case "lastName_desc":
                    users = users.OrderByDescending(b => b.LastName);
                    break;
                default:
                    break;
            }

            int pageSize = 12;
            return this.View(await PaginatedList<UsersAllViewModel>.CreateAsync(users, pageNumber ?? 1, pageSize));
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
