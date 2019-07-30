namespace TrainConnected.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using TrainConnected.Common;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Workouts;

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
        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        public async Task<IActionResult> MyCreated()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var workouts = await this.workoutsService.GetMyCreatedAsync(userId);
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

            if (workout == null)
            {
                return this.NotFound();
            }

            return this.View(workout);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        public async Task<IActionResult> Create()
        {
            this.ViewData["Activities"] = await this.GetAllWorkoutActivitiesAsSelectListItems();

            var paymentMethodsByType = await this.GetAllPaymentMethodsByTypeAsync();

            this.ViewData["paymentMethodsInAdvance"] = paymentMethodsByType["paymentMethodsInAdvance"];
            this.ViewData["paymentMethodsOnSite"] = paymentMethodsByType["paymentMethodsOnSite"];

            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutCreateInputModel workoutCreateInputModel, List<string> acceptedPaymentMethods)
        {
            if (!this.ModelState.IsValid || acceptedPaymentMethods.Count == 0)
            {
                // TODO: Add Error Message for accepted payment methods
                this.ViewData["Activities"] = await this.GetAllWorkoutActivitiesAsSelectListItems();

                var paymentMethodsByType = await this.GetAllPaymentMethodsByTypeAsync();

                this.ViewData["paymentMethodsInAdvance"] = paymentMethodsByType["paymentMethodsInAdvance"];
                this.ViewData["paymentMethodsOnSite"] = paymentMethodsByType["paymentMethodsOnSite"];

                return this.View(workoutCreateInputModel);
            }

            workoutCreateInputModel.PaymentMethods = acceptedPaymentMethods;
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await this.workoutsService.CreateAsync(workoutCreateInputModel, userId);

            return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        public async Task<IActionResult> Cancel(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var workout = await this.workoutsService.GetCancelDetailsAsync(id);

            if (workout == null)
            {
                return this.NotFound();
            }

            return this.View(workout);

        }

        [HttpPost, ActionName("Cancel")]
        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            await this.workoutsService.CancelAsync(id);

            return this.RedirectToAction(nameof(this.Find));
        }

        [NonAction]
        private async Task<IEnumerable<SelectListItem>> GetAllWorkoutActivitiesAsSelectListItems()
        {
            var activities = await this.workoutActivitiesService.GetAllAsync();

            var selectList = new List<SelectListItem>();

            foreach (var element in activities)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.Name,
                    Text = element.Name,
                });
            }

            return selectList;
        }

        [NonAction]
        private async Task<Dictionary<string, List<string>>> GetAllPaymentMethodsByTypeAsync()
        {
            var paymentMethods = await this.paymentMethodsService.GetAllAsync();

            var paymentMethodsInAdvance = paymentMethods
                .Where(pia => pia.PaymentInAdvance == true)
                .Select(n => n.Name)
                .ToList();

            var paymentMethodsOnSite = paymentMethods
                .Where(pia => pia.PaymentInAdvance == false)
                .Select(n => n.Name)
                .ToList();

            var paymentMethodsByType = new Dictionary<string, List<string>>()
            {
                { "paymentMethodsInAdvance", paymentMethodsInAdvance },
                { "paymentMethodsOnSite", paymentMethodsOnSite },
            };

            return paymentMethodsByType;
        }
    }
}
