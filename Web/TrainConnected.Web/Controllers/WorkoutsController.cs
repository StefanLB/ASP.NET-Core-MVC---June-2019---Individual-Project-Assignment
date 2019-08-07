namespace TrainConnected.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;

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
        public async Task<IActionResult> Find()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var workouts = await this.workoutsService.GetAllUpcomingAsync(userId);
            return this.View(workouts);
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var workouts = await this.workoutsService.GetMyAsync(userId);
            return this.View(workouts);
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
