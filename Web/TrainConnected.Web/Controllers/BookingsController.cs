namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Bookings;
    using TrainConnected.Web.ViewModels.Workouts;

    public class BookingsController : BaseController
    {
        private readonly IBookingsService bookingsService;
        private readonly IWorkoutsService workoutsService;
        private readonly IPaymentMethodsService paymentMethodsService;

        public BookingsController(IBookingsService bookingsService, IWorkoutsService workoutsService, IPaymentMethodsService paymentMethodsService)
        {
            this.bookingsService = bookingsService;
            this.workoutsService = workoutsService;
            this.paymentMethodsService = paymentMethodsService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await this.bookingsService.GetAllAsync(userId);
            return this.View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> AllHistory()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await this.bookingsService.GetAllHistoryAsync(userId);
            return this.View(bookings);
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
                var booking = await this.bookingsService.GetDetailsAsync(id, userId);
                return this.View(booking);
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
        public async Task<IActionResult> DetailsByWorkoutId(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var booking = await this.bookingsService.GetDetailsByWorkoutIdAsync(id, userId);
                return this.RedirectToAction(nameof(this.Details), new { id = booking.Id });
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            WorkoutDetailsViewModel workout;

            try
            {
                workout = await this.workoutsService.GetDetailsAsync(id, userId);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }

            var paymentMethodsByType = await this.GetApplicablePaymentMethodsByTypeAsync(workout);

            this.ViewData["workout"] = workout;
            this.ViewData["paymentMethodsInAdvance"] = paymentMethodsByType["paymentMethodsInAdvance"];
            this.ViewData["paymentMethodsOnSite"] = paymentMethodsByType["paymentMethodsOnSite"];

            return this.View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateInputModel bookingCreateInputModel)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!this.ModelState.IsValid)
            {
                WorkoutDetailsViewModel workout;

                try
                {
                    workout = await this.workoutsService.GetDetailsAsync(bookingCreateInputModel.WorkoutId, userId);
                }
                catch (NullReferenceException)
                {
                    return this.NotFound();
                }

                var paymentMethodsByType = await this.GetApplicablePaymentMethodsByTypeAsync(workout);

                this.ViewData["workout"] = workout;
                this.ViewData["paymentMethodsInAdvance"] = paymentMethodsByType["paymentMethodsInAdvance"];
                this.ViewData["paymentMethodsOnSite"] = paymentMethodsByType["paymentMethodsOnSite"];

                return this.View(bookingCreateInputModel);
            }

            try
            {
                var result = await this.bookingsService.CreateAsync(bookingCreateInputModel, userId);

                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var booking = await this.bookingsService.GetDetailsAsync(id, userId);

                if (DateTime.UtcNow > booking.WorkoutTime || booking.PaymentMethodPaymentInAdvance == true)
                {
                    return this.BadRequest();
                }

                return this.View(booking);
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

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            try
            {
                await this.bookingsService.CancelAsync(id);
                return this.RedirectToAction(nameof(this.All));
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }

        [NonAction]
        private async Task<Dictionary<string, List<string>>> GetApplicablePaymentMethodsByTypeAsync(WorkoutDetailsViewModel workout)
        {
            var paymentMethods = await this.paymentMethodsService.GetAllAsync();

            var paymentMethodsInAdvance = paymentMethods
                .Where(pia => pia.PaymentInAdvance == true && workout.AcceptedPaymentMethods.Contains(pia.Name))
                .Select(n => n.Name)
                .ToList();

            var paymentMethodsOnSite = paymentMethods
                .Where(pia => pia.PaymentInAdvance == false && workout.AcceptedPaymentMethods.Contains(pia.Name))
                .Select(n => n.Name)
                .ToList();

            var paymentMethodsByType = new Dictionary<string, List<string>>()
            {
                { "paymentMethodsInAdvance", paymentMethodsInAdvance },
                { "paymentMethodsOnSite", paymentMethodsOnSite },
            };

            return paymentMethodsByType;
        }
    }
}
