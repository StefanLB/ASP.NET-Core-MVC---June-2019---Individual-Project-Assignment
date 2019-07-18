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
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels.WorkoutActivities;

    public class WorkoutActivitiesService : IWorkoutActivitiesService
    {
        private readonly IRepository<WorkoutActivity> workoutActivitiesRepository;

        public WorkoutActivitiesService(IRepository<WorkoutActivity> workoutActivitiesRepository)
        {
            this.workoutActivitiesRepository = workoutActivitiesRepository;
        }

        public async Task<WorkoutActivityDetailsViewModel> CreateAsync(WorkoutActivityCreateInputModel workoutActivityCreateInputModel)
        {
            // TODO: create additional method, to be used in Edit as well
            var checkActivityExists = this.workoutActivitiesRepository.All()
                .FirstOrDefault(x => x.Name == workoutActivityCreateInputModel.Name);

            if (checkActivityExists != null)
            {
                return AutoMapper.Mapper.Map<WorkoutActivityDetailsViewModel>(checkActivityExists);
            }

            var workoutActivity = new WorkoutActivity
            {
                Name = workoutActivityCreateInputModel.Name,
                Description = workoutActivityCreateInputModel.Description
            };

            await this.workoutActivitiesRepository.AddAsync(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivityDetailsViewModel = AutoMapper.Mapper.Map<WorkoutActivityDetailsViewModel>(workoutActivity);
            return workoutActivityDetailsViewModel;
        }

        public async Task DeleteAsync(string id)
        {
            var workoutActivity = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (workoutActivity == null)
            {
                // TODO: catch exception and redirect appropriately
                throw new NullReferenceException();
            }

            workoutActivity.IsDeleted = true;
            this.workoutActivitiesRepository.Update(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkoutActivitiesAllViewModel>> GetAllAsync()
        {
            var activities = await this.workoutActivitiesRepository.All()
                .To<WorkoutActivitiesAllViewModel>()
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return activities;
        }

        public async Task<WorkoutActivityDetailsViewModel> GetDetailsAsync(string id)
        {
            var workoutActivity = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            var workoutActivityDetailsViewModel = AutoMapper.Mapper.Map<WorkoutActivityDetailsViewModel>(workoutActivity);
            return workoutActivityDetailsViewModel;
        }

        public async Task<WorkoutActivityEditInputModel> GetEditDetailsAsync(string id)
        {
            var workoutActivity = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            var workoutActivityEditInputModel = AutoMapper.Mapper.Map<WorkoutActivityEditInputModel>(workoutActivity);
            return workoutActivityEditInputModel;
        }

        public async Task UpdateAsync(WorkoutActivityEditInputModel workoutActivityEditInputModel)
        {
            var workoutActivity = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == workoutActivityEditInputModel.Id);

            if (workoutActivity == null)
            {
                // TODO: catch exception and redirect appropriately
                throw new NullReferenceException();
            }

            var existingActivityWithSameName = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Name == workoutActivityEditInputModel.Name);

            if (existingActivityWithSameName != null)
            {
                if (existingActivityWithSameName.Id != workoutActivity.Id)
                {
                    // TODO: catch exception and redirect appropriately
                    throw new InvalidOperationException("Activity with same name is already registered!");
                }
            }

            workoutActivity.Name = workoutActivityEditInputModel.Name;
            workoutActivity.Description = workoutActivityEditInputModel.Description;

            this.workoutActivitiesRepository.Update(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();
        }
    }
}
