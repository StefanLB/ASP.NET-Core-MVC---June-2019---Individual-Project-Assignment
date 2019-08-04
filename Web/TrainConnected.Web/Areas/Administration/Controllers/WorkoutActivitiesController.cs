namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.WorkoutActivities;

    public class WorkoutActivitiesController : AdministrationController
    {
        private readonly IWorkoutActivitiesService workoutActivitiesService;

        private readonly ICloudinaryService cloudinaryService;

        public WorkoutActivitiesController(IWorkoutActivitiesService workoutActivitiesService, ICloudinaryService cloudinaryService)
        {
            this.workoutActivitiesService = workoutActivitiesService;
            this.cloudinaryService = cloudinaryService;
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
                return this.NotFound();
            }

            var activity = await this.workoutActivitiesService.GetDetailsAsync(id);
            return this.View(activity);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(WorkoutActivityCreateInputModel workoutActivityCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(workoutActivityCreateInputModel);
            }

            string pictureUrl = await this.cloudinaryService.UploadPictureAsync(
                workoutActivityCreateInputModel.Icon,
                workoutActivityCreateInputModel.Name);

            var workoutActivityServiceModel = AutoMapper.Mapper.Map<WorkoutActivityServiceModel>(workoutActivityCreateInputModel);
            workoutActivityServiceModel.Icon = pictureUrl;

            var result = await this.workoutActivitiesService.CreateAsync(workoutActivityServiceModel);

            return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var workoutActivity = await this.workoutActivitiesService.GetEditDetailsAsync(id);
            return this.View(workoutActivity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkoutActivityEditInputModel workoutActivityEditInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(workoutActivityEditInputModel);
            }

            await this.workoutActivitiesService.UpdateAsync(workoutActivityEditInputModel);
            return this.RedirectToAction(nameof(this.All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var workoutActivity = await this.workoutActivitiesService.GetDetailsAsync(id);
            return this.View(workoutActivity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await this.workoutActivitiesService.DeleteAsync(id);
            return this.RedirectToAction(nameof(this.All));
        }
    }
}
