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
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            return null;

        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            return null;

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

            return this.RedirectToAction(nameof(this.Details), id);
        }

        [HttpGet]
        public async Task<IActionResult> Remove(string id)
        {
            return null;

        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(string id)
        {
            return null;

        }

    }
}
