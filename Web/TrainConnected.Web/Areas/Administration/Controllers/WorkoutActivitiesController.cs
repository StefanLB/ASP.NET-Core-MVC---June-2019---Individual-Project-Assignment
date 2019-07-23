namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Common;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Data;

    public class WorkoutActivitiesController : AdministrationController
    {
        private readonly IWorkoutActivitiesService workoutActivitiesService;

        public WorkoutActivitiesController(IWorkoutActivitiesService workoutActivitiesService)
        {
            this.workoutActivitiesService = workoutActivitiesService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var activities = await this.workoutActivitiesService.GetAllAsync();

            return this.View(activities);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await this.workoutActivitiesService.GetDetailsAsync(id);

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(WorkoutActivityCreateInputModel workoutActivityCreateInputModel)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.workoutActivitiesService.CreateAsync(workoutActivityCreateInputModel);

                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }

            return this.View(workoutActivityCreateInputModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutActivity = await this.workoutActivitiesService.GetEditDetailsAsync(id);

            if (workoutActivity == null)
            {
                return NotFound();
            }

            return View(workoutActivity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkoutActivityEditInputModel workoutActivityEditInputModel)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    await this.workoutActivitiesService.UpdateAsync(workoutActivityEditInputModel);

                    return this.RedirectToAction(nameof(All));
                }
                catch (InvalidOperationException)
                {
                    return View(workoutActivityEditInputModel);
                }

            }

            return View(workoutActivityEditInputModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutActivity = await this.workoutActivitiesService.GetDetailsAsync(id);

            if (workoutActivity == null)
            {
                return NotFound();
            }

            return View(workoutActivity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await this.workoutActivitiesService.DeleteAsync(id);

            return this.RedirectToAction(nameof(All));
        }
    }
}
