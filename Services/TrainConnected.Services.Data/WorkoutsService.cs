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
        private readonly IRepository<PaymentMethod> paymentMethodsRepository;
        private readonly IRepository<WorkoutsPaymentMethods> workoutsPaymentsMethodsRepository;

        public WorkoutsService(IRepository<Workout> workoutsRepository, IRepository<TrainConnectedUser> usersRepository, IRepository<WorkoutActivity> workoutActivitiesRepository, IRepository<TrainConnectedUsersWorkouts> usersWorkoutsRepository, IRepository<PaymentMethod> paymentMethodsRepository, IRepository<WorkoutsPaymentMethods> workoutsPaymentsMethodsRepository)
        {
            this.workoutsRepository = workoutsRepository;
            this.usersRepository = usersRepository;
            this.workoutActivityRepository = workoutActivitiesRepository;
            this.usersWorkoutsRepository = usersWorkoutsRepository;
            this.paymentMethodsRepository = paymentMethodsRepository;
            this.workoutsPaymentsMethodsRepository = workoutsPaymentsMethodsRepository;
        }

        public async Task<IEnumerable<WorkoutsHomeViewModel>> GetAllUpcomingHomeAsync()
        {
            var workouts = await this.workoutsRepository.All()
                .Where(t => t.Time > DateTime.UtcNow)
                .To<WorkoutsHomeViewModel>()
                .OrderBy(x => x.Time)
                .ThenBy(x => x.ActivityName)
                .ToArrayAsync();

            return workouts;
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
                .OrderBy(x => x.Time)
                .ThenBy(x => x.ActivityName)
                .ThenByDescending(x => x.CreatedOn)
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

        public async Task<WorkoutDetailsViewModel> CreateAsync(WorkoutCreateInputModel workoutCreateInputModel, string userId)
        {
            var workoutActivity = this.workoutActivityRepository.All()
                .FirstOrDefault(x => x.Name == workoutCreateInputModel.Activity);

            if (workoutActivity == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Workout.NullReferenceActivityName, workoutCreateInputModel.Activity));
            }

            var user = this.usersRepository.All()
                .FirstOrDefault(x => x.Id == userId);

            var paymentMethods = this.paymentMethodsRepository.All()
                .Where(n => workoutCreateInputModel.PaymentMethods.Contains(n.Name))
                .ToArray();

            if (paymentMethods == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Workout.NullReferencePaymentMethodName));
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

            foreach (var paymentMethod in paymentMethods)
            {
                await this.workoutsPaymentsMethodsRepository.AddAsync(new WorkoutsPaymentMethods
                {
                    WorkoutId = workout.Id,
                    PaymentMethodId = paymentMethod.Id,
                });
            }

            await this.workoutsPaymentsMethodsRepository.SaveChangesAsync();

            var workoutDetailsViewModel = AutoMapper.Mapper.Map<WorkoutDetailsViewModel>(workout);
            return workoutDetailsViewModel;
        }

        public async Task<WorkoutDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var workoutDetailsViewModel = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .To<WorkoutDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (workoutDetailsViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Workout.NullReferenceWorkoutId, id));
            }

            var paymentMethodsIds = await this.workoutsPaymentsMethodsRepository.All()
                .Where(x => x.WorkoutId == id)
                .Select(pm => pm.PaymentMethodId)
                .ToArrayAsync();

            var paymentMethodsNames = await this.paymentMethodsRepository.All()
                .Where(pm => paymentMethodsIds.Contains(pm.Id))
                .Select(n => n.Name)
                .ToArrayAsync();

            workoutDetailsViewModel.AcceptedPaymentMethods = paymentMethodsNames;

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

        public async Task<WorkoutCancelViewModel> GetCancelDetailsAsync(string id, string userId)
        {
            var workout = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .To<WorkoutCancelViewModel>()
                .FirstOrDefaultAsync();

            if (workout == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Workout.NullReferenceWorkoutId, id));
            }

            var createdByUserId = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .Select(c => c.CoachId)
                .FirstOrDefaultAsync();

            if (createdByUserId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Workout.ArgumentUserIdMismatch, userId));
            }

            // TODO: Extract method for GetDetails and GetCancelDetails for the payment methods
            var paymentMethodsIds = await this.workoutsPaymentsMethodsRepository.All()
                .Where(x => x.WorkoutId == id)
                .Select(pm => pm.PaymentMethodId)
                .ToArrayAsync();

            var paymentMethodsNames = await this.paymentMethodsRepository.All()
                .Where(pm => paymentMethodsIds.Contains(pm.Id))
                .Select(n => n.Name)
                .ToArrayAsync();

            workout.AcceptedPaymentMethods = paymentMethodsNames;
            return workout;
        }

        public async Task CancelAsync(string id, string userId)
        {
            var workout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (workout == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Workout.NullReferenceWorkoutId, id));
            }

            if (workout.CoachId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Workout.ArgumentUserIdMismatch, userId));
            }

            if (workout.CurrentlySignedUp == 0)
            {
                workout.IsDeleted = true;
                this.workoutsRepository.Update(workout);
                await this.workoutsRepository.SaveChangesAsync();
            }
            else
            {
                // If this code block is reached, user tried to bypass front end validation
                throw new InvalidOperationException(string.Format(ServiceConstants.Workout.WorkoutCancelCriteriaNotMet));
            }
        }
    }
}
