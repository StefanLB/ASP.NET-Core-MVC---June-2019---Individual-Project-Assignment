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
    using TrainConnected.Web.InputModels.WorkoutActivities;
    using TrainConnected.Web.ViewModels.WorkoutActivities;

    public class WorkoutActivitiesService : IWorkoutActivitiesService
    {
        private readonly IRepository<WorkoutActivity> workoutActivitiesRepository;

        public WorkoutActivitiesService(IRepository<WorkoutActivity> workoutActivitiesRepository)
        {
            this.workoutActivitiesRepository = workoutActivitiesRepository;
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

            if (workoutActivity == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.WorkoutActivity.NullReferenceActivityId, id));
            }

            var workoutActivityDetailsViewModel = AutoMapper.Mapper.Map<WorkoutActivityDetailsViewModel>(workoutActivity);
            return workoutActivityDetailsViewModel;
        }

        public async Task<WorkoutActivityDetailsViewModel> CreateAsync(WorkoutActivityServiceModel workoutActivityServiceModel)
        {
            var checkActivityExists = this.workoutActivitiesRepository.All()
                .FirstOrDefault(x => x.Name == workoutActivityServiceModel.Name);

            if (checkActivityExists != null)
            {
                return AutoMapper.Mapper.Map<WorkoutActivityDetailsViewModel>(checkActivityExists);
            }

            var workoutActivity = new WorkoutActivity
            {
                Name = workoutActivityServiceModel.Name,
                Description = workoutActivityServiceModel.Description,
                Icon = workoutActivityServiceModel.Icon,
            };

            await this.workoutActivitiesRepository.AddAsync(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();

            var workoutActivityDetailsViewModel = AutoMapper.Mapper.Map<WorkoutActivityDetailsViewModel>(workoutActivity);
            return workoutActivityDetailsViewModel;
        }

        public async Task<WorkoutActivityEditInputModel> GetEditDetailsAsync(string id)
        {
            var workoutActivity = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (workoutActivity == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.WorkoutActivity.NullReferenceActivityId, id));
            }

            var workoutActivityEditInputModel = AutoMapper.Mapper.Map<WorkoutActivityEditInputModel>(workoutActivity);
            return workoutActivityEditInputModel;
        }

        public async Task UpdateAsync(WorkoutActivityEditInputModel workoutActivityEditInputModel)
        {
            var workoutActivity = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == workoutActivityEditInputModel.Id);

            if (workoutActivity == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.WorkoutActivity.NullReferenceActivityId, workoutActivityEditInputModel.Id));
            }

            var existingActivityWithSameName = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Name == workoutActivityEditInputModel.Name);

            if (existingActivityWithSameName != null)
            {
                if (existingActivityWithSameName.Id != workoutActivity.Id)
                {
                    throw new InvalidOperationException(string.Format(ServiceConstants.WorkoutActivity.SameNameActivityExists));
                }
            }

            workoutActivity.Name = workoutActivityEditInputModel.Name;
            workoutActivity.Description = workoutActivityEditInputModel.Description;

            this.workoutActivitiesRepository.Update(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var workoutActivity = await this.workoutActivitiesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (workoutActivity == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.WorkoutActivity.NullReferenceActivityId, id));
            }

            workoutActivity.IsDeleted = true;
            this.workoutActivitiesRepository.Update(workoutActivity);
            await this.workoutActivitiesRepository.SaveChangesAsync();
        }
    }
}
