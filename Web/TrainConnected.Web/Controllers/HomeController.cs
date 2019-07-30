namespace TrainConnected.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;

    public class HomeController : BaseController
    {
        private readonly IWorkoutsService workoutsService;

        public HomeController(IWorkoutsService workoutsService)
        {
            this.workoutsService = workoutsService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var workouts = await this.workoutsService.GetAllUpcomingHomeAsync();

            return this.View(workouts);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();
    }
}
