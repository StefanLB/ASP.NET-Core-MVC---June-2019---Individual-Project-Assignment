namespace TrainConnected.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Certificates;

    [Authorize]
    public class CertificatesController : BaseController
    {
        private readonly ICertificatesService certificatesService;

        public CertificatesController(ICertificatesService certificatesService)
        {
            this.certificatesService = certificatesService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var username = this.User.Identity.Name;
            var certificates = await this.certificatesService.GetAllAsync(username);
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
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CertificateCreateInputModel certificateCreateInputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this.certificatesService.CreateAsync(certificateCreateInputModel);

                return this.RedirectToAction(nameof(this.All));
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

            var certificate = await this.certificatesService.GetDetailsAsync(id);

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
    }
}
