namespace TrainConnected.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels.WorkoutActivities;

    [Authorize]
    public class WorkoutsController : BaseController
    {
        private readonly IWorkoutsService workoutsService;
        private readonly IWorkoutActivitiesService workoutActivitiesService;

        public WorkoutsController(IWorkoutsService workoutsService, IWorkoutActivitiesService workoutActivitiesService)
        {
            this.workoutsService = workoutsService;
            this.workoutActivitiesService = workoutActivitiesService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var workouts = await this.workoutsService.GetAllAsync();
            return this.View(workouts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var workout = await this.workoutsService.GetDetailsAsync(id);

            if (workout == null)
            {
                return this.NotFound();
            }

            return this.View(workout);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var activities = await this.workoutActivitiesService.GetAllAsync();
            var activitiesSelectList = await this.GetAllWorkoutActivitiesAsSelectListItems(activities);
            this.ViewData["Activities"] = activitiesSelectList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutCreateInputModel workoutCreateInputModel)
        {
            if (this.ModelState.IsValid)
            {
                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var result = await this.workoutsService.CreateAsync(workoutCreateInputModel, userId);

                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }

            return this.View(workoutCreateInputModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var workout = await this.workoutsService.GetDetailsAsync(id);

            if (workout == null)
            {
                return this.NotFound();
            }

            return this.View(workout);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await this.workoutsService.DeleteAsync(id);

            return this.RedirectToAction(nameof(this.All));
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
                    Text = element.Name
                });
            }

            return selectList;
        }
    }
}
