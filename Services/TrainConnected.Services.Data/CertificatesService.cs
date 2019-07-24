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
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.Certificates;

    public class CertificatesService : ICertificatesService
    {
        private readonly IRepository<Certificate> certificatesRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IRepository<WorkoutActivity> workoutActivityRepository;

        public CertificatesService(IRepository<Certificate> certificatesRepository, IRepository<TrainConnectedUser> usersRepository, IRepository<WorkoutActivity> workoutActivityRepository)
        {
            this.certificatesRepository = certificatesRepository;
            this.usersRepository = usersRepository;
            this.workoutActivityRepository = workoutActivityRepository;
        }

        public async Task UpdateAsync(CertificateEditInputModel certificateEditInputModel)
        {
            var certificate = await this.certificatesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == certificateEditInputModel.Id);

            if (certificate == null)
            {
                // TODO: catch exception and redirect appropriately
                throw new NullReferenceException();
            }

            var workoutActivity = await this.workoutActivityRepository.All()
                .FirstOrDefaultAsync(x => x.Name == certificateEditInputModel.ActivityName);

            if (workoutActivity == null)
            {
                // TODO: catch exception and redirect appropriately
                throw new NullReferenceException();
            }

            certificate.ActivityId = workoutActivity.Id;
            certificate.Activity = workoutActivity;
            certificate.IssuedBy = certificateEditInputModel.IssuedBy;
            certificate.IssuedOn = certificateEditInputModel.IssuedOn;
            certificate.ExpiresOn = certificateEditInputModel.ExpiresOn;
            certificate.Description = certificateEditInputModel.Description;

            this.certificatesRepository.Update(certificate);
            await this.certificatesRepository.SaveChangesAsync();
        }

        public async Task<CertificateDetailsViewModel> CreateAsync(CertificateCreateInputModel certificatesCreateInputModel, string username)
        {
            var workoutActivity = this.workoutActivityRepository.All()
                .FirstOrDefault(x => x.Name == certificatesCreateInputModel.Activity);

            if (workoutActivity == null)
            {
                throw new NullReferenceException();
            }

            var user = this.usersRepository.All()
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                throw new NullReferenceException();
            }

            var certificate = new Certificate
            {
                Activity = workoutActivity,
                IssuedBy = certificatesCreateInputModel.IssuedBy,
                IssuedOn = certificatesCreateInputModel.IssuedOn,
                ExpiresOn = certificatesCreateInputModel.ExpiresOn,
                Description = certificatesCreateInputModel.Description,
                TrainConnectedUser = user,
                TrainConnectedUserId = user.Id,
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            var certificateDetailsViewModel = AutoMapper.Mapper.Map<CertificateDetailsViewModel>(certificate);
            return certificateDetailsViewModel;
        }

        public async Task DeleteAsync(string id)
        {
            var certificate = await this.certificatesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (certificate == null)
            {
                // TODO: catch exception and redirect appropriately
                throw new NullReferenceException();
            }

            certificate.IsDeleted = true;
            this.certificatesRepository.Update(certificate);
            await this.certificatesRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string userId)
        {
            var certificates = await this.certificatesRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .To<CertificatesAllViewModel>()
                .OrderBy(x => x.ActivityName)
                .ThenByDescending(x => x.IssuedOn)
                .ToArrayAsync();

            return certificates;
        }

        public async Task<CertificateDetailsViewModel> GetDetailsAsync(string id)
        {
            var certificate = await this.certificatesRepository.All()
                .Where(x => x.Id == id)
                .To<CertificateDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (certificate == null)
            {
                throw new InvalidOperationException();
            }

            return certificate;
        }

        public async Task<CertificateEditInputModel> GetEditDetailsAsync(string id)
        {
            var certificate = await this.certificatesRepository.All()
                .Where(x => x.Id == id)
                .To<CertificateEditInputModel>()
                .FirstOrDefaultAsync();

            if (certificate == null)
            {
                throw new InvalidOperationException();
            }

            return certificate;
        }
    }
}
