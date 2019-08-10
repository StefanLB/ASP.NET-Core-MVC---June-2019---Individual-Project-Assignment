namespace TrainConnected.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.Helpers;
    using TrainConnected.Web.InputModels.Bookings;
    using TrainConnected.Web.ViewModels.Bookings;
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
        public async Task<IActionResult> All(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            this.ViewData["CurrentSort"] = sortOrder;
            this.ViewData["TimeSortParm"] = sortOrder == "Time" ? "time_desc" : "Time";
            this.ViewData["ActivitySortParm"] = sortOrder == "Activity" ? "activity_desc" : "Activity";
            this.ViewData["LocationSortParm"] = sortOrder == "Location" ? "location_desc" : "Location";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            this.ViewData["CurrentFilter"] = searchString;

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await this.bookingsService.GetAllAsync(userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(w => w.WorkoutActivityName.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "Time":
                    bookings = bookings.OrderBy(w => w.WorkoutTime);
                    break;
                case "time_desc":
                    bookings = bookings.OrderByDescending(w => w.WorkoutTime);
                    break;
                case "Activity":
                    bookings = bookings.OrderBy(w => w.WorkoutActivityName);
                    break;
                case "activity_desc":
                    bookings = bookings.OrderByDescending(w => w.WorkoutActivityName);
                    break;
                case "Location":
                    bookings = bookings.OrderBy(w => w.WorkoutLocation);
                    break;
                case "location_desc":
                    bookings = bookings.OrderByDescending(w => w.WorkoutLocation);
                    break;
                default:
                    break;
            }

            int pageSize = 12;
            return this.View(await PaginatedList<BookingsAllViewModel>.CreateAsync(bookings, pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> AllHistory(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            this.ViewData["CurrentSort"] = sortOrder;
            this.ViewData["TimeSortParm"] = sortOrder == "Time" ? "time_desc" : "Time";
            this.ViewData["ActivitySortParm"] = sortOrder == "Activity" ? "activity_desc" : "Activity";
            this.ViewData["LocationSortParm"] = sortOrder == "Location" ? "location_desc" : "Location";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            this.ViewData["CurrentFilter"] = searchString;

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await this.bookingsService.GetAllHistoryAsync(userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(w => w.WorkoutActivityName.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "Time":
                    bookings = bookings.OrderBy(w => w.WorkoutTime);
                    break;
                case "time_desc":
                    bookings = bookings.OrderByDescending(w => w.WorkoutTime);
                    break;
                case "Activity":
                    bookings = bookings.OrderBy(w => w.WorkoutActivityName);
                    break;
                case "activity_desc":
                    bookings = bookings.OrderByDescending(w => w.WorkoutActivityName);
                    break;
                case "Location":
                    bookings = bookings.OrderBy(w => w.WorkoutLocation);
                    break;
                case "location_desc":
                    bookings = bookings.OrderByDescending(w => w.WorkoutLocation);
                    break;
                default:
                    break;
            }

            int pageSize = 12;
            return this.View(await PaginatedList<BookingsAllViewModel>.CreateAsync(bookings, pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var booking = await this.bookingsService.GetDetailsAsync(id, userId);
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
            return this.RedirectToAction(nameof(this.Details), new { id = booking.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            WorkoutDetailsViewModel workout;

            workout = await this.workoutsService.GetDetailsAsync(id, userId);

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

                workout = await this.workoutsService.GetDetailsAsync(bookingCreateInputModel.WorkoutId, userId);

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

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var booking = await this.bookingsService.GetDetailsAsync(id, userId);

            if (DateTime.UtcNow.ToLocalTime() > booking.WorkoutTime || booking.PaymentMethodPaymentInAdvance == true)
            {
                return this.BadRequest();
            }

            return this.View(booking);
        }

        [HttpPost]
        [ActionName("Cancel")]
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
