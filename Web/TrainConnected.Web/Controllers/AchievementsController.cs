namespace TrainConnected.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;

    public class AchievementsController : Controller
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

            var achievement = await this.achievementsService.GetDetailsAsync(id);

            if (achievement == null)
            {
                return this.NotFound();
            }

            return this.View(achievement);
        }
    }
}
