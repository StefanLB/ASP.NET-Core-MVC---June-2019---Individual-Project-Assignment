namespace TrainConnected.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.Workouts;
    using TrainConnected.Web.ViewModels.Workouts;

    public class WorkoutsService : IWorkoutsService
    {
        private readonly IRepository<Workout> workoutsRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<WorkoutActivity> workoutActivityRepository;
        private readonly IRepository<TrainConnectedUsersWorkouts> usersWorkoutsRepository;

        public WorkoutsService(IRepository<Workout> workoutsRepository, IRepository<TrainConnectedUser> usersRepository, IRepository<WorkoutActivity> workoutActivityRepository, IRepository<TrainConnectedUsersWorkouts> usersWorkoutsRepository)
        {
            this.workoutsRepository = workoutsRepository;
            this.usersRepository = usersRepository;
            this.workoutActivityRepository = workoutActivityRepository;
            this.usersWorkoutsRepository = usersWorkoutsRepository;
        }

        public async Task<WorkoutDetailsViewModel> CreateAsync(WorkoutCreateInputModel workoutCreateInputModel, string userId)
        {
            var workoutActivity = this.workoutActivityRepository.All()
                .FirstOrDefault(x => x.Name == workoutCreateInputModel.Activity);

            if (workoutActivity == null)
            {
                throw new NullReferenceException();
            }

            var user = this.usersRepository.All()
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException();
            }

            var workout = new Workout
            {
                ActivityId = workoutActivity.Id,
                Activity = workoutActivity,
                Coach = user,
                CoachId = user.Id,
                Time = workoutCreateInputModel.Time,
                Location = workoutCreateInputModel.Location,
                Duration = workoutCreateInputModel.Duration,
                Price = workoutCreateInputModel.Price,
                Notes = workoutCreateInputModel.Notes,
                MaxParticipants = workoutCreateInputModel.MaxParticipants,
            };

            await this.workoutsRepository.AddAsync(workout);
            await this.workoutsRepository.SaveChangesAsync();

            var workoutDetailsViewModel = AutoMapper.Mapper.Map<WorkoutDetailsViewModel>(workout);
            return workoutDetailsViewModel;
        }

        public async Task CancelAsync(string id)
        {
            var workout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (workout == null)
            {
                // TODO: catch exception and redirect appropriately
                throw new NullReferenceException();
            }

            if (workout.CurrentlySignedUp == 0)
            {
                // TODO: Return warning in case workout cannot be deleted
                workout.IsDeleted = true;
                this.workoutsRepository.Update(workout);
                await this.workoutsRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WorkoutsAllViewModel>> GetAllUpcomingAsync(string userId)
        {
            var userWorkoutsIds = await this.usersWorkoutsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .Select(w => w.WorkoutId)
                .ToArrayAsync();

            var workouts = await this.workoutsRepository.All()
                .Where(t => t.Time > DateTime.UtcNow)
                .Where(w => !userWorkoutsIds.Contains(w.Id))
                .Where(c => c.CoachId != userId)
                .To<WorkoutsAllViewModel>()
                .OrderByDescending(x => x.Time)
                .ThenBy(x => x.ActivityName)
                .ThenByDescending(x => x.CreatedOn)
                .ToArrayAsync();

            return workouts;
        }

        public async Task<IEnumerable<WorkoutsHomeViewModel>> GetAllUpcomingHomeAsync()
        {
            var workouts = await this.workoutsRepository.All()
                .Where(t => t.Time > DateTime.UtcNow)
                .To<WorkoutsHomeViewModel>()
                .OrderByDescending(x => x.Time)
                .ThenBy(x => x.ActivityName)
                .ToArrayAsync();

            return workouts;
        }

        public async Task<IEnumerable<WorkoutsAllViewModel>> GetMyAsync(string userId)
        {
            var userWorkoutsIds = await this.usersWorkoutsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .Select(w => w.WorkoutId)
                .ToArrayAsync();

            var workouts = await this.workoutsRepository.All()
                .Where(w => userWorkoutsIds.Contains(w.Id))
                .To<WorkoutsAllViewModel>()
                .OrderByDescending(x => x.Time)
                .ThenBy(x => x.ActivityName)
                .ThenByDescending(x => x.CreatedOn)
                .ToArrayAsync();

            return workouts;
        }

        public async Task<IEnumerable<WorkoutsAllViewModel>> GetMyCreatedAsync(string userId)
        {
            var workouts = await this.workoutsRepository.All()
                .Where(w => w.CoachId == userId)
                .To<WorkoutsAllViewModel>()
                .OrderByDescending(x => x.Time)
                .ThenBy(x => x.ActivityName)
                .ThenByDescending(x => x.CreatedOn)
                .ToArrayAsync();

            return workouts;
        }

        public async Task<WorkoutDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var workoutDetailsViewModel = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .To<WorkoutDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (workoutDetailsViewModel == null)
            {
                throw new InvalidOperationException();
            }

            workoutDetailsViewModel.IsBookableByUser = false;

            var workoutFromDb = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var userWorkoutBookings = await this.usersWorkoutsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .Select(w => w.WorkoutId)
                .ToArrayAsync();

            /*
             * Check if:
             * 1. Workout has not begun;
             * 2. User is not the coach;
             * 3. Workout is not fully booked;
             * 4. user has not yet booked the workout.
             */
            if (workoutFromDb.Time > DateTime.UtcNow)
            {
                if (workoutFromDb.CoachId != userId)
                {
                    if (workoutDetailsViewModel.BookingsCount < workoutDetailsViewModel.MaxParticipants)
                    {
                        if (!userWorkoutBookings.Any(w => w == workoutFromDb.Id))
                        {
                            workoutDetailsViewModel.IsBookableByUser = true;
                        }
                    }
                }
            }

            return workoutDetailsViewModel;
        }

        public async Task<WorkoutDetailsViewModel> GetBookingDetailsAsync(string id)
        {
            var workoutDetailsViewModel = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .To<WorkoutDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (workoutDetailsViewModel == null)
            {
                throw new InvalidOperationException();
            }

            workoutDetailsViewModel.IsBookableByUser = true;

            return workoutDetailsViewModel;
        }

        public async Task<WorkoutCancelViewModel> GetCancelDetailsAsync(string id)
        {
            var workout = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .To<WorkoutCancelViewModel>()
                .FirstOrDefaultAsync();

            if (workout == null)
            {
                throw new InvalidOperationException();
            }

            return workout;
        }
    }
}
