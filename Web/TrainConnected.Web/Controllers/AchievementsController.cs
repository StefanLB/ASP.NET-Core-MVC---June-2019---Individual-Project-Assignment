namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;

    public class AchievementsController : BaseController
    {
        private readonly IAchievementsService achievementsService;

        public AchievementsController(IAchievementsService achievementsService)
        {
            this.achievementsService = achievementsService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await this.achievementsService.CheckForAchievementsAsync(userId);
            var achievements = await this.achievementsService.GetAllAsync(userId);
            return this.View(achievements);
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
                var achievement = await this.achievementsService.GetDetailsAsync(id, userId);
                return this.View(achievement);
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
