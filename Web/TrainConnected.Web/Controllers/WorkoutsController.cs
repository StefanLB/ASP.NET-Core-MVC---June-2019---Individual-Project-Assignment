namespace TrainConnected.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels.WorkoutActivities;

    public class WorkoutsController : BaseController
    {
        private readonly IWorkoutsService workoutsService;
        private readonly IWorkoutActivitiesService workoutActivitiesService;

        public WorkoutsController(IWorkoutsService workoutsService, IWorkoutActivitiesService workoutActivitiesService)
        {
            this.workoutsService = workoutsService;
            this.workoutActivitiesService = workoutActivitiesService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var workouts = await this.workoutsService.GetAllAsync();
            return this.View(workouts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var workout = await this.workoutsService.GetDetailsAsync(id);

            if (workout == null)
            {
                return this.NotFound();
            }

            return this.View(workout);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var activities = await this.workoutActivitiesService.GetAllAsync();
            var activitiesSelectList = await this.GetAllWorkoutActivitiesAsSelectListItems(activities);
            this.ViewData["Activities"] = activitiesSelectList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutCreateInputModel workoutCreateInputModel)
        {
            if (this.ModelState.IsValid)
            {
                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var result = await this.workoutsService.CreateAsync(workoutCreateInputModel, userId);

                return this.RedirectToAction(nameof(this.Details), new { id = result.Id });
            }

            return this.View(workoutCreateInputModel);
        }

        //// GET: Workouts/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var workout = await _context.Workouts.FindAsync(id);
        //    if (workout == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ActivityId"] = new SelectList(_context.WorkoutActivities, "Id", "Id", workout.ActivityId);
        //    ViewData["CoachId"] = new SelectList(_context.Users, "Id", "Id", workout.CoachId);
        //    return View(workout);
        //}

        //// POST: Workouts/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("ActivityId,CoachId,Time,Location,Duration,Price,Notes,MaxParticipants,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Workout workout)
        //{
        //    if (id != workout.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(workout);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!WorkoutExists(workout.Id))
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
        //    ViewData["ActivityId"] = new SelectList(_context.WorkoutActivities, "Id", "Id", workout.ActivityId);
        //    ViewData["CoachId"] = new SelectList(_context.Users, "Id", "Id", workout.CoachId);
        //    return View(workout);
        //}

        //// GET: Workouts/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var workout = await _context.Workouts
        //        .Include(w => w.Activity)
        //        .Include(w => w.Coach)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (workout == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(workout);
        //}

        //// POST: Workouts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var workout = await _context.Workouts.FindAsync(id);
        //    _context.Workouts.Remove(workout);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool WorkoutExists(string id)
        //{
        //    return _context.Workouts.Any(e => e.Id == id);
        //}

        [NonAction]
        private async Task<IEnumerable<SelectListItem>> GetAllWorkoutActivitiesAsSelectListItems(IEnumerable<WorkoutActivitiesAllViewModel> activities)
        {
            var selectList = new List<SelectListItem>();

            foreach (var element in activities)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.Name,
                    Text = element.Name
                });
            }

            return selectList;
        }
    }
}
