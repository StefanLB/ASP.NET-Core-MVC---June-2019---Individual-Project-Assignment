namespace TrainConnected.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.Helpers;
    using TrainConnected.Web.ViewModels.Workouts;

    [Authorize]
    public class WorkoutsController : BaseController
    {
        private readonly IWorkoutsService workoutsService;
        private readonly IWorkoutActivitiesService workoutActivitiesService;
        private readonly IPaymentMethodsService paymentMethodsService;

        public WorkoutsController(IWorkoutsService workoutsService, IWorkoutActivitiesService workoutActivitiesService, IPaymentMethodsService paymentMethodsService)
        {
            this.workoutsService = workoutsService;
            this.workoutActivitiesService = workoutActivitiesService;
            this.paymentMethodsService = paymentMethodsService;
        }

        // Displays all upcoming workouts for which the user has not signed up
        [HttpGet]
        public async Task<IActionResult> Find(string sortOrder, string currentFilter, string searchString, int? pageNumber)
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

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var workouts = await this.workoutsService.GetAllUpcomingAsync(userId);

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

            return this.View(await PaginatedList<WorkoutsAllViewModel>.CreateAsync(workouts, pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> My(string sortOrder, string currentFilter, string searchString, int? pageNumber)
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

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var workouts = await this.workoutsService.GetMyAsync(userId);

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

            return this.View(await PaginatedList<WorkoutsAllViewModel>.CreateAsync(workouts, pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var workout = await this.workoutsService.GetDetailsAsync(id, userId);
            return this.View(workout);
        }
    }
}
