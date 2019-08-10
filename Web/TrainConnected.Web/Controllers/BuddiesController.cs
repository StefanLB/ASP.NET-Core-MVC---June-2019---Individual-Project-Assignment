namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.Helpers;
    using TrainConnected.Web.ViewModels.Buddies;

    public class BuddiesController : BaseController
    {
        private readonly IBuddiesService buddiesService;

        public BuddiesController(IBuddiesService buddiesService)
        {
            this.buddiesService = buddiesService;
        }

        // TODO: Add pending buddy request and buddies will be connected after the buddy request is accepted by the other user
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

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var buddies = await this.buddiesService.GetAllAsync(userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                buddies = buddies.Where(b => b.UserName.ToLower().Contains(searchString.ToLower()) ||
                                             b.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                             b.LastName.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "UserName":
                    buddies = buddies.OrderBy(b => b.UserName);
                    break;
                case "userName_desc":
                    buddies = buddies.OrderByDescending(b => b.UserName);
                    break;
                case "FirstName":
                    buddies = buddies.OrderBy(b => b.FirstName);
                    break;
                case "firstName_desc":
                    buddies = buddies.OrderByDescending(b => b.FirstName);
                    break;
                case "LastName":
                    buddies = buddies.OrderBy(b => b.LastName);
                    break;
                case "lastName_desc":
                    buddies = buddies.OrderByDescending(b => b.LastName);
                    break;
                default:
                    break;
            }

            int pageSize = 12;
            return this.View(await PaginatedList<BuddiesAllViewModel>.CreateAsync(buddies, pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Find(string sortOrder, string currentFilter, string searchString, int? pageNumber)
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

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var nonBuddyUsers = await this.buddiesService.FindAllAsync(userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                nonBuddyUsers = nonBuddyUsers.Where(b => b.UserName.ToLower().Contains(searchString.ToLower()) ||
                                                         b.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                                         b.LastName.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "UserName":
                    nonBuddyUsers = nonBuddyUsers.OrderBy(b => b.UserName);
                    break;
                case "userName_desc":
                    nonBuddyUsers = nonBuddyUsers.OrderByDescending(b => b.UserName);
                    break;
                case "FirstName":
                    nonBuddyUsers = nonBuddyUsers.OrderBy(b => b.FirstName);
                    break;
                case "firstName_desc":
                    nonBuddyUsers = nonBuddyUsers.OrderByDescending(b => b.FirstName);
                    break;
                case "LastName":
                    nonBuddyUsers = nonBuddyUsers.OrderBy(b => b.LastName);
                    break;
                case "lastName_desc":
                    nonBuddyUsers = nonBuddyUsers.OrderByDescending(b => b.LastName);
                    break;
                default:
                    break;
            }

            int pageSize = 12;
            return this.View(await PaginatedList<BuddiesAllViewModel>.CreateAsync(nonBuddyUsers, pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var buddyDetails = await this.buddiesService.GetDetailsAsync(id, userId);
            return this.View(buddyDetails);
        }

        [HttpGet]
        public async Task<IActionResult> CoachDetails(string coachUserName)
        {
            if (coachUserName == null)
            {
                return this.NotFound();
            }

            var coachDetails = await this.buddiesService.GetCoachDetailsAsync(coachUserName);
            return this.View(coachDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Add(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.buddiesService.AddAsync(id, userId);
            return this.RedirectToAction(nameof(this.Find));
        }

        [HttpGet]
        public async Task<IActionResult> Remove(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var buddyDetails = await this.buddiesService.GetDetailsAsync(id, userId);
            return this.View(buddyDetails);
        }

        [HttpPost,]
        [ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.buddiesService.RemoveAsync(id, userId);
            return this.RedirectToAction(nameof(this.All));
        }
    }
}
