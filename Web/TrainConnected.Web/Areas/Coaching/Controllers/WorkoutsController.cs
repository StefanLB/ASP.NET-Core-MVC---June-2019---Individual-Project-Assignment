namespace TrainConnected.Web.Areas.Coaching.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using TrainConnected.Common;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.Helpers;
    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels.Workouts;

    public class WorkoutsController : CoachingController
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

        [HttpGet]
        public async Task<IActionResult> MyCreated(string sortOrder, string currentFilter, string searchString, int? pageNumber)
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
            var workouts = await this.workoutsService.GetMyCreatedAsync(userId);

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
            if (acceptedPaymentMethods.Count == 0)
            {
                this.ModelState.AddModelError("PaymentMethods", ModelConstants.Workout.PaymentMethodsError);
            }

            if (!this.ModelState.IsValid)
            {
                this.ViewData["Activities"] = await this.GetAllWorkoutActivitiesAsSelectListItems();

                var paymentMethodsByType = await this.GetAllPaymentMethodsByTypeAsync();

                this.ViewData["paymentMethodsInAdvance"] = paymentMethodsByType["paymentMethodsInAdvance"];
                this.ViewData["paymentMethodsOnSite"] = paymentMethodsByType["paymentMethodsOnSite"];

                return this.View(workoutCreateInputModel);
            }

            workoutCreateInputModel.PaymentMethods = acceptedPaymentMethods;
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = await this.workoutsService.CreateAsync(workoutCreateInputModel, userId);
            return this.RedirectToAction("Details", "Workouts", new { area = string.Empty, id = result.Id });
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        public async Task<IActionResult> Cancel(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var workout = await this.workoutsService.GetCancelDetailsAsync(id, userId);

            if (DateTime.UtcNow.ToLocalTime() > workout.Time || workout.BookingsCount != 0)
            {
                return this.BadRequest();
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

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.workoutsService.CancelAsync(id, userId);
            return this.RedirectToAction(nameof(this.MyCreated));
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