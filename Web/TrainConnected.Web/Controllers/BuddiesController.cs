namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;

    public class BuddiesController : BaseController
    {
        private readonly IBuddiesService buddiesService;

        public BuddiesController(IBuddiesService buddiesService)
        {
            this.buddiesService = buddiesService;
        }

        // TODO: Add pending buddy request and buddies will be connected after the buddy request is accepted by the other user
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

            try
            {
                var buddyDetails = await this.buddiesService.GetDetailsAsync(id, userId);
                return this.View(buddyDetails);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CoachDetails(string coachUserName)
        {
            if (coachUserName == null)
            {
                return this.NotFound();
            }

            try
            {
                var coachDetails = await this.buddiesService.GetCoachDetailsAsync(coachUserName);
                return this.View(coachDetails);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                await this.buddiesService.AddAsync(id, userId);
                return this.RedirectToAction(nameof(this.Find));
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Remove(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var buddyDetails = await this.buddiesService.GetDetailsAsync(id, userId);
                return this.View(buddyDetails);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                await this.buddiesService.RemoveAsync(id, userId);
                return this.RedirectToAction(nameof(this.All));
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
        }
    }
}
