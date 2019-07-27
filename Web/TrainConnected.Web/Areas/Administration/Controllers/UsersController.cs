namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;

    public class UsersController : AdministrationController
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var users = await this.usersService.GetAllAsync();

            return this.View(users);
        }
    }
}
