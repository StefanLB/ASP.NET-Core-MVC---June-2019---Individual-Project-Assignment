namespace TrainConnected.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

        public WorkoutsService(IRepository<Workout> workoutsRepository, IRepository<TrainConnectedUser> usersRepository, IRepository<WorkoutActivity> workoutActivityRepository)
        {
            this.workoutsRepository = workoutsRepository;
            this.usersRepository = usersRepository;
            this.workoutActivityRepository = workoutActivityRepository;
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

        public async Task<IEnumerable<WorkoutsAllViewModel>> GetAllAsync()
        {
            var certificates = await this.workoutsRepository.All()
                .To<WorkoutsAllViewModel>()
                .OrderBy(x => x.ActivityName)
                .ThenByDescending(x => x.CreatedOn)
                .ToArrayAsync();

            return certificates;
        }

        public async Task<WorkoutDetailsViewModel> GetDetailsAsync(string id)
        {
            var workout = await this.workoutsRepository.All()
                .Where(x => x.Id == id)
                .To<WorkoutDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (workout == null)
            {
                throw new InvalidOperationException();
            }

            return workout;
        }
    }
}
