namespace TrainConnected.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.Certificates;

    using AutoMap = AutoMapper;

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

        public async Task<CertificateDetailsViewModel> UpdateAsync(CertificateEditInputModel certificateEditInputModel)
        {
            //    try
            //    {
            //        _context.Update(certificate);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!CertificateExists(certificate.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            throw new System.NotImplementedException();
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
                TrainConnectedUserId = user.Id
            };

            await this.certificatesRepository.AddAsync(certificate);
            await this.certificatesRepository.SaveChangesAsync();

            var certificateDetailsViewModel = AutoMapper.Mapper.Map<CertificateDetailsViewModel>(certificate);
            return certificateDetailsViewModel;
        }

        public async Task DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string username)
        {
            var userCertificates = this.usersRepository.All()
                .FirstOrDefault(x => x.UserName == username)
                .Certificates
                .Select(x => x.Id)
                .ToArray();

            var certificates = await this.certificatesRepository.All()
                .Where(x => userCertificates.Contains(x.Id))
                .To<CertificatesAllViewModel>()
                .OrderBy(x => x.Activity)
                .ThenByDescending(x => x.IssuedOn)
                .ToArrayAsync();

            return certificates;
        }

        public async Task<CertificateDetailsViewModel> GetDetailsAsync(string id)
        {
            var certificate = await this.certificatesRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            var certificateDetailsViewModel = AutoMapper.Mapper.Map<CertificateDetailsViewModel>(certificate);
            return certificateDetailsViewModel;
        }

        public bool CertificateExists(string id)
        {
            throw new System.NotImplementedException();
        }

    }
}
