namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;

        public DashboardController(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}
