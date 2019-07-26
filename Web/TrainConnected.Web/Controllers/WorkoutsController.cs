namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Common;
    using TrainConnected.Data;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.PaymentMethods;
    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels.WorkoutActivities;

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
            var activities = await this.workoutActivitiesService.GetAllAsync();
            var activitiesSelectList = await this.GetAllWorkoutActivitiesAsSelectListItems(activities);
            this.ViewData["Activities"] = activitiesSelectList;

            var paymentMethods = await this.paymentMethodsService.GetAllAsync();
            var paymentMethodsNames = paymentMethods.Select(x => x.Name).ToList();

            this.ViewData["PaymentMethods"] = paymentMethodsNames;

            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutCreateInputModel workoutCreateInputModel, List<string> AcceptedPaymentMethods)
        {
            if (this.ModelState.IsValid && AcceptedPaymentMethods.Count > 0)
            {
                workoutCreateInputModel.PaymentMethods = AcceptedPaymentMethods;

                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var result = await this.workoutsService.CreateAsync(workoutCreateInputModel, userId);

                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }

            return this.View(workoutCreateInputModel);
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

        [Authorize(Roles = GlobalConstants.CoachRoleName)]
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(string id)
        {
            await this.workoutsService.CancelAsync(id);

            return this.RedirectToAction(nameof(this.Find));
        }

        [NonAction]
        private async Task<IEnumerable<SelectListItem>> GetAllWorkoutActivitiesAsSelectListItems(IEnumerable<WorkoutActivitiesAllViewModel> activities)
        {
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
    }
}
