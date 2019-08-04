namespace TrainConnected.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.Helpers;
    using TrainConnected.Web.ViewModels.Workouts;

    public class HomeController : BaseController
    {
        private readonly IWorkoutsService workoutsService;

        public HomeController(IWorkoutsService workoutsService)
        {
            this.workoutsService = workoutsService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            this.ViewData["CurrentSort"] = sortOrder;
            this.ViewData["TimeSortParm"] = sortOrder == "Time" ? "time_desc" : "Time";
            this.ViewData["ActivitySortParm"] = sortOrder == "Activity" ? "activity_desc" : "Activity";
            this.ViewData["LocationSortParm"] = sortOrder == "Location" ? "location_desc" : "Location";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            this.ViewData["CurrentFilter"] = searchString;

            var workouts = await this.workoutsService.GetAllUpcomingHomeAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                workouts = workouts.Where(w => w.ActivityName.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "Time":
                    workouts = workouts.OrderBy(w => w.Time);
                    break;
                case "time_desc":
                    workouts = workouts.OrderByDescending(w => w.Time);
                    break;
                case "Activity":
                    workouts = workouts.OrderBy(w => w.ActivityName);
                    break;
                case "activity_desc":
                    workouts = workouts.OrderByDescending(w => w.ActivityName);
                    break;
                case "Location":
                    workouts = workouts.OrderBy(w => w.Location);
                    break;
                case "location_desc":
                    workouts = workouts.OrderByDescending(w => w.Location);
                    break;
                default:
                    break;
            }

            int pageSize = 12;
            return this.View(await PaginatedList<WorkoutsHomeViewModel>.CreateAsync(workouts, pageNumber ?? 1, pageSize));
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return this.View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();
    }
}
