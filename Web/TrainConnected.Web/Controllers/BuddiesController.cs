namespace TrainConnected.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrainConnected.Services.Data.Contracts;

    [Authorize]
    public class BuddiesController : BaseController
    {
        private readonly IBuddiesService buddiesService;

        public BuddiesController(IBuddiesService buddiesService)
        {
            this.buddiesService = buddiesService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var buddies = await this.buddiesService.GetAllAsync(userId);

            return this.View(buddies);
        }

        [HttpGet]
        public async Task<IActionResult> Find()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var nonBuddyUsers = await this.buddiesService.FindAllAsync(userId);

            return this.View(nonBuddyUsers);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.buddiesService.AddAsync(id, userId);

            return this.RedirectToAction(nameof(this.All));
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

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(string id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.buddiesService.RemoveAsync(id, userId);

            return this.RedirectToAction(nameof(this.All), userId);
        }

    }
}
