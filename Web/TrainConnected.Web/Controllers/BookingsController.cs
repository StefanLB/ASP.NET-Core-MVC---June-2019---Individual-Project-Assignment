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
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Bookings;

    [Authorize]
    public class BookingsController : BaseController
    {
        private readonly IBookingsService bookingsService;

        public BookingsController(IBookingsService bookingsService)
        {
            this.bookingsService = bookingsService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await this.bookingsService.GetAllAsync(userId);
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
        public async Task<IActionResult> Create(string id)
        {
            var workout = await this.bookingsService.GetWorkoutByIdAsync(id);
            var paymentMethods = await this.GetAllPaymentMethodsAsSelectListItems();

            ViewData["Workout"] = workout;
            ViewData["PaymentMethods"] = paymentMethods;

            return View();
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

            return this.View(booking);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(string id)
        {
            await this.bookingsService.CancelAsync(id);

            return this.RedirectToAction(nameof(this.All));
        }

        [NonAction]
        private async Task<IEnumerable<SelectListItem>> GetAllPaymentMethodsAsSelectListItems()
        {
            var selectList = new List<SelectListItem>();

            foreach (PaymentMethod pm in (PaymentMethod[])Enum.GetValues(typeof(PaymentMethod)))
            {
                selectList.Add(new SelectListItem
                {
                    Value = pm.ToString(),
                    Text = pm.ToString(),
                });
            }

            return selectList;
        }
    }
}
