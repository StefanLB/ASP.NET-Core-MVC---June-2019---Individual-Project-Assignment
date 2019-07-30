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

            var booking = await this.bookingsService.GetDetailsAsync(id);

            if (booking == null)
            {
                return this.NotFound();
            }

            return this.View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsByWorkoutId(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var booking = await this.bookingsService.GetDetailsByWorkoutIdAsync(id, userId);

            if (booking == null)
            {
                return this.NotFound();
            }

            return this.RedirectToAction(nameof(this.Details), new { id = booking.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var workout = await this.workoutsService.GetDetailsAsync(id, userId);
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
                // TODO: workout Id may be tampered, to be checked before passed to Service or catch the exception here
                var workout = await this.workoutsService.GetDetailsAsync(bookingCreateInputModel.WorkoutId, userId);
                var paymentMethodsByType = await this.GetApplicablePaymentMethodsByTypeAsync(workout);

                this.ViewData["workout"] = workout;
                this.ViewData["paymentMethodsInAdvance"] = paymentMethodsByType["paymentMethodsInAdvance"];
                this.ViewData["paymentMethodsOnSite"] = paymentMethodsByType["paymentMethodsOnSite"];

                return this.View(bookingCreateInputModel);
            }

            var result = await this.bookingsService.CreateAsync(bookingCreateInputModel, userId);

            return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var booking = await this.bookingsService.GetDetailsAsync(id);

            if (booking == null)
            {
                return this.NotFound();
            }

            if (DateTime.UtcNow > booking.WorkoutTime || booking.PaymentMethodPaymentInAdvance == true)
            {
                // TODO: user tried to cancel a booking which cannot be cancelled: should redirect to forbidden or something else
                return this.NotFound();
            }

            return this.View(booking);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            await this.bookingsService.CancelAsync(id);

            return this.RedirectToAction(nameof(this.All));
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
