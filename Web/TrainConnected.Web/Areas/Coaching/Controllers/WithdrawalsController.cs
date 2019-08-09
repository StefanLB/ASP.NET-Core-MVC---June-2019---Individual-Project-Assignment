namespace TrainConnected.Web.Areas.Coaching.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Withdrawals;

    public class WithdrawalsController : CoachingController
    {
        private readonly IWithdrawalsService withdrawalsService;

        public WithdrawalsController(IWithdrawalsService withdrawalsService)
        {
            this.withdrawalsService = withdrawalsService;
        }

        [HttpGet]
        public async Task<IActionResult> All(string searchString)
        {
            this.ViewData["CurrentFilter"] = searchString;

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var withdrawals = await this.withdrawalsService.GetAllAsync(userId);
            this.ViewData["userBalance"] = await this.withdrawalsService.GetUserBalanceAsync(userId);
            this.ViewData["pendingWithdrawals"] = await this.withdrawalsService.GetUserPendingWithdrawalsBalance(userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                withdrawals = withdrawals.Where(w => w.Id.Contains(searchString));
            }

            return this.View(withdrawals);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            this.ViewData["userBalance"] = await this.withdrawalsService.GetUserBalanceAsync(userId);
            this.ViewData["pendingWithdrawals"] = await this.withdrawalsService.GetUserPendingWithdrawalsBalance(userId);

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WithdrawalCreateInputModel withdrawalCreateInputModel)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!this.ModelState.IsValid)
            {
                this.ViewData["userBalance"] = await this.withdrawalsService.GetUserBalanceAsync(userId);
                this.ViewData["pendingWithdrawals"] = await this.withdrawalsService.GetUserPendingWithdrawalsBalance(userId);

                return this.View(withdrawalCreateInputModel);
            }

            await this.withdrawalsService.CreateAsync(withdrawalCreateInputModel, userId);
            return this.RedirectToAction(nameof(this.All));
        }
    }
}
