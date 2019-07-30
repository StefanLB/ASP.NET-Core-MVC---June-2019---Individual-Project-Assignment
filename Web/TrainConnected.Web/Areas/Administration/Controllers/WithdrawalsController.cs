namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Withdrawals;

    public class WithdrawalsController : AdministrationController
    {
        private readonly IWithdrawalsService withdrawalsService;

        public WithdrawalsController(IWithdrawalsService withdrawalsService)
        {
            this.withdrawalsService = withdrawalsService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var withdrawals = await this.withdrawalsService.GetAllAdminAsync();

            return this.View(withdrawals);
        }

        [HttpGet]
        public async Task<IActionResult> Process(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            try
            {
                var withdrawal = await this.withdrawalsService.GetForProcessingAsync(id);
                this.ViewData["withdrawal"] = withdrawal;
                return this.View();
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(string id, WithdrawalProcessInputModel withdrawalProcessInputModel)
        {
            if (id != withdrawalProcessInputModel.Id)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(withdrawalProcessInputModel);
            }

            try
            {
                var processedByUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await this.withdrawalsService.ProcessAsync(withdrawalProcessInputModel, processedByUserId);
                return this.RedirectToAction(nameof(this.All));
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }
    }
}
