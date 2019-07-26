namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Bookings;

    [Authorize]
    public class BookingsController : BaseController
    {
        private readonly IBookingsService bookingsService;
        private readonly IWorkoutsService workoutsService;

        public BookingsController(IBookingsService bookingsService, IWorkoutsService workoutsService)
        {
            this.bookingsService = bookingsService;
            this.workoutsService = workoutsService;
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

            this.ViewData["Workout"] = workout;

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateInputModel bookingCreateInputModel)
        {
            if (this.ModelState.IsValid)
            {
                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var result = await this.bookingsService.CreateAsync(bookingCreateInputModel, userId);

                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }

            return this.View(bookingCreateInputModel);
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
            await this.bookingsService.CancelAsync(id);

            return this.RedirectToAction(nameof(this.All));
        }
    }
}
