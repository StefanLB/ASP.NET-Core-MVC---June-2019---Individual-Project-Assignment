using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainConnected.Data;
using TrainConnected.Data.Models;

namespace TrainConnected.Web.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly TrainConnectedDbContext _context;

        public WorkoutsController(TrainConnectedDbContext context)
        {
            _context = context;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            var trainConnectedDbContext = _context.Workouts.Include(w => w.Activity).Include(w => w.Coach);
            return View(await trainConnectedDbContext.ToListAsync());
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Activity)
                .Include(w => w.Coach)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            ViewData["ActivityId"] = new SelectList(_context.WorkoutActivities, "Id", "Id");
            ViewData["CoachId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Workouts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityId,CoachId,Time,Location,Duration,Price,Notes,MaxParticipants,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityId"] = new SelectList(_context.WorkoutActivities, "Id", "Id", workout.ActivityId);
            ViewData["CoachId"] = new SelectList(_context.Users, "Id", "Id", workout.CoachId);
            return View(workout);
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            ViewData["ActivityId"] = new SelectList(_context.WorkoutActivities, "Id", "Id", workout.ActivityId);
            ViewData["CoachId"] = new SelectList(_context.Users, "Id", "Id", workout.CoachId);
            return View(workout);
        }

        // POST: Workouts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ActivityId,CoachId,Time,Location,Duration,Price,Notes,MaxParticipants,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Workout workout)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityId"] = new SelectList(_context.WorkoutActivities, "Id", "Id", workout.ActivityId);
            ViewData["CoachId"] = new SelectList(_context.Users, "Id", "Id", workout.CoachId);
            return View(workout);
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Activity)
                .Include(w => w.Coach)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExists(string id)
        {
            return _context.Workouts.Any(e => e.Id == id);
        }
    }
}
