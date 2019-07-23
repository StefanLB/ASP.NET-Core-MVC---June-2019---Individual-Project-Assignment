namespace TrainConnected.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrainConnected.Services.Data.Contracts;

    public class HomeController : BaseController
    {
        private readonly IWorkoutsService workoutsService;

        public HomeController(IWorkoutsService workoutsService)
        {
            this.workoutsService = workoutsService;
        }

        public async Task<IActionResult> Index()
        {
            var workouts = await this.workoutsService.GetAllUpcomingHomeAsync();

            return this.View(workouts);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();
    }
}
