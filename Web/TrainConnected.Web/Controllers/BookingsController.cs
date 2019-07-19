using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainConnected.Data;
using TrainConnected.Data.Models;
using TrainConnected.Data.Models.Enums;
using TrainConnected.Services.Data.Contracts;
using TrainConnected.Web.InputModels.Bookings;

namespace TrainConnected.Web.Controllers
{
    [Authorize]
    public class BookingsController : BaseController
    {
        private readonly IBookingsService bookingsService;

        public BookingsController(IBookingsService bookingsService)
        {
            this.bookingsService = bookingsService;
        }

        //[HttpGet]
        //public async Task<IActionResult> All()
        //{
        //    var trainConnectedDbContext = _context.Bookings.Include(b => b.TrainConnectedUser).Include(b => b.Workout);
        //    return View(await trainConnectedDbContext.ToListAsync());
        //}

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
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

        // GET: Bookings/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var booking = await _context.Bookings.FindAsync(id);
        //    if (booking == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["TrainConnectedUserId"] = new SelectList(_context.Users, "Id", "Id", booking.TrainConnectedUserId);
        //    ViewData["WorkoutId"] = new SelectList(_context.Workouts, "Id", "Id", booking.WorkoutId);
        //    return View(booking);
        //}

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("TrainConnectedUserId,PaymentMethod,Price,WorkoutId,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Booking booking)
        //{
        //    if (id != booking.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(booking);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BookingExists(booking.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["TrainConnectedUserId"] = new SelectList(_context.Users, "Id", "Id", booking.TrainConnectedUserId);
        //    ViewData["WorkoutId"] = new SelectList(_context.Workouts, "Id", "Id", booking.WorkoutId);
        //    return View(booking);
        //}

        // GET: Bookings/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var booking = await _context.Bookings
        //        .Include(b => b.TrainConnectedUser)
        //        .Include(b => b.Workout)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (booking == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(booking);
        //}

        // POST: Bookings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var booking = await _context.Bookings.FindAsync(id);
        //    _context.Bookings.Remove(booking);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

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
