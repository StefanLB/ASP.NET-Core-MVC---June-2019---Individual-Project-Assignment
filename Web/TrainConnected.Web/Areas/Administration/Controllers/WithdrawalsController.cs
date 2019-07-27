namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using TrainConnected.Services.Data.Contracts;

    public class WithdrawalsController : AdministrationController
    {
        private readonly IWithdrawalsService withdrawalsService;

        public WithdrawalsController(IWithdrawalsService withdrawalsService)
        {
            this.withdrawalsService = withdrawalsService;
        }


    }
}
