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
    using TrainConnected.Common;
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.WorkoutActivities;

    [Authorize(Roles = GlobalConstants.CoachRoleName)]
    public class CertificatesController : BaseController
    {
        private readonly ICertificatesService certificatesService;
        private readonly IWorkoutActivitiesService workoutActivitiesService;

        public CertificatesController(ICertificatesService certificatesService, IWorkoutActivitiesService workoutActivitiesService)
        {
            this.certificatesService = certificatesService;
            this.workoutActivitiesService = workoutActivitiesService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var certificates = await this.certificatesService.GetAllAsync(userId);
            return this.View(certificates);
        }

        // TODO: If a user tries to view details of a different user, redirect to All?
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var certificate = await this.certificatesService.GetDetailsAsync(id);

            if (certificate == null)
            {
                return this.NotFound();
            }

            return this.View(certificate);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // TODO: failing validation does not bring back the dropdown menu
            // TODO: validation for expires on to be after issued on
            // TODO: validation for issued on to be in the past

            var activities = await this.workoutActivitiesService.GetAllAsync();
            var activitiesSelectList = await this.GetAllWorkoutActivitiesAsSelectListItems(activities);
            this.ViewData["Activities"] = activitiesSelectList;

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CertificateCreateInputModel certificateCreateInputModel)
        {
            if (this.ModelState.IsValid)
            {
                var username = this.User.Identity.Name;

                var result = await this.certificatesService.CreateAsync(certificateCreateInputModel, username);

                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }

            return this.View(certificateCreateInputModel);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var activities = await this.workoutActivitiesService.GetAllAsync();
            var activitiesSelectList = await this.GetAllWorkoutActivitiesAsSelectListItems(activities);
            this.ViewData["Activities"] = activitiesSelectList;

            var certificate = await this.certificatesService.GetEditDetailsAsync(id);

            if (certificate == null)
            {
                return this.NotFound();
            }

            return this.View(certificate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CertificateEditInputModel certificateEditInputModel)
        {
            if (id != certificateEditInputModel.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                await this.certificatesService.UpdateAsync(certificateEditInputModel);

                return this.RedirectToAction(nameof(this.All));
            }

            return this.View(certificateEditInputModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var certificate = await this.certificatesService.GetDetailsAsync(id);

            if (certificate == null)
            {
                return this.NotFound();
            }

            return this.View(certificate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await this.certificatesService.DeleteAsync(id);

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
