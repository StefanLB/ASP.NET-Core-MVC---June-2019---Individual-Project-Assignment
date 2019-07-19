namespace TrainConnected.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Withdrawals;

    public class WithdrawalsController : Controller
    {
        private readonly IWithdrawalsService withdrawalsService;

        public WithdrawalsController(IWithdrawalsService withdrawalsService)
        {
            this.withdrawalsService = withdrawalsService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var withdrawals = await this.withdrawalsService.GetAllAsync(userId);
            return this.View(withdrawals);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            this.ViewData["userBalance"] = await this.withdrawalsService.GetUserBalanceAsync(userId);

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WithdrawalCreateInputModel withdrawalCreateInputModel)
        {
            if (this.ModelState.IsValid)
            {
                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                await this.withdrawalsService.CreateAsync(withdrawalCreateInputModel, userId);

                return this.RedirectToAction(nameof(All));
            }

            return this.View(withdrawalCreateInputModel);
        }
    }
}
