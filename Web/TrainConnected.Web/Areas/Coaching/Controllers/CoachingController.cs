namespace TrainConnected.Web.Areas.Coaching.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Common;
    using TrainConnected.Web.Controllers;

    [Authorize(Roles = GlobalConstants.CoachRoleName)]
    [Area("Coaching")]
    public abstract class CoachingController : BaseController
    {
    }
}
