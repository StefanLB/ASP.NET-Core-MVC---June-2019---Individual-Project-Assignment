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

        public async Task<CertificateDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var certificate = await this.certificatesRepository.All()
                .Where(x => x.Id == id)
                .To<CertificateDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (certificate == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Certificate.NullReferenceCertificateId, id));
            }

            var issuedToUserId = await this.certificatesRepository.All()
                .Where(x => x.Id == id)
                .Select(x => x.TrainConnectedUserId)
                .FirstOrDefaultAsync();

            if (issuedToUserId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Certificate.ArgumentUserIdMismatch, userId));
            }

            return certificate;
        }

        public async Task<CertificateDetailsViewModel> CreateAsync(CertificateCreateInputModel certificatesCreateInputModel, string userId)
        {
            var workoutActivity = this.workoutActivityRepository.All()
                .FirstOrDefault(x => x.Name == certificatesCreateInputModel.Activity);

            if (workoutActivity == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Certificate.NullReferenceWorkoutActivityName, certificatesCreateInputModel.Activity));
            }

            var user = this.usersRepository.All()
                .FirstOrDefault(x => x.Id == userId);

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

        public async Task<CertificateEditInputModel> GetEditDetailsAsync(string id, string userId)
        {
            var certificate = await this.certificatesRepository.All()
                .Where(x => x.Id == id)
                .To<CertificateEditInputModel>()
                .FirstOrDefaultAsync();

            if (certificate == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Certificate.NullReferenceCertificateId, id));
            }

            var issuedToUserId = await this.certificatesRepository.All()
                .Where(x => x.Id == id)
                .Select(x => x.TrainConnectedUserId)
                .FirstOrDefaultAsync();

            if (issuedToUserId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Certificate.ArgumentUserIdMismatch, userId));
            }

            return certificate;
        }

        public async Task UpdateAsync(CertificateEditInputModel certificateEditInputModel, string userId)
        {
            var certificate = await this.certificatesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == certificateEditInputModel.Id);

            if (certificate == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Certificate.NullReferenceCertificateId, certificateEditInputModel.Id));
            }

            var issuedToUserId = await this.certificatesRepository.All()
                .Where(x => x.Id == userId)
                .Select(x => x.TrainConnectedUserId)
                .FirstOrDefaultAsync();

            if (issuedToUserId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Certificate.ArgumentUserIdMismatch, userId));
            }

            var workoutActivity = await this.workoutActivityRepository.All()
                .FirstOrDefaultAsync(x => x.Name == certificateEditInputModel.ActivityName);

            if (workoutActivity == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Certificate.NullReferenceWorkoutActivityName, certificateEditInputModel.ActivityName));
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

        public async Task DeleteAsync(string id, string userId)
        {
            var certificate = await this.certificatesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (certificate == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Certificate.NullReferenceCertificateId, id));
            }

            var issuedToUserId = await this.certificatesRepository.All()
                .Where(x => x.Id == userId)
                .Select(x => x.TrainConnectedUserId)
                .FirstOrDefaultAsync();

            if (issuedToUserId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Certificate.ArgumentUserIdMismatch, userId));
            }

            certificate.IsDeleted = true;
            this.certificatesRepository.Update(certificate);
            await this.certificatesRepository.SaveChangesAsync();
        }
    }
}
