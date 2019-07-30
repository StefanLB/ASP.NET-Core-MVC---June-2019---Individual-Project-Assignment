namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using TrainConnected.Common;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Certificates;

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

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var certificate = await this.certificatesService.GetDetailsAsync(id, userId);
                return this.View(certificate);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            this.ViewData["Activities"] = await this.GetAllWorkoutActivitiesAsSelectListItems();

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CertificateCreateInputModel certificateCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewData["Activities"] = await this.GetAllWorkoutActivitiesAsSelectListItems();

                return this.View(certificateCreateInputModel);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var result = await this.certificatesService.CreateAsync(certificateCreateInputModel, userId);
                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                this.ViewData["Activities"] = await this.GetAllWorkoutActivitiesAsSelectListItems();
                var certificate = await this.certificatesService.GetEditDetailsAsync(id, userId);
                return this.View(certificate);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CertificateEditInputModel certificateEditInputModel)
        {
            if (id != certificateEditInputModel.Id)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                this.ViewData["Activities"] = await this.GetAllWorkoutActivitiesAsSelectListItems();
                return this.View(certificateEditInputModel);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                await this.certificatesService.UpdateAsync(certificateEditInputModel, userId);
                return this.RedirectToAction(nameof(this.All));
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var certificate = await this.certificatesService.GetDetailsAsync(id, userId);
                return this.View(certificate);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                await this.certificatesService.DeleteAsync(id, userId);
                return this.RedirectToAction(nameof(this.All));
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
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
    }
}
