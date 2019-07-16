namespace TrainConnected.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.Certificates;

    public class CertificatesService : ICertificatesService
    {
        private readonly IRepository<Certificate> certificatesRepository;

        public CertificatesService(IRepository<Certificate> certificatesRepository)
        {
            this.certificatesRepository = certificatesRepository;
        }

        public Task<CertificateDetailsViewModel> UpdateAsync(CertificateEditInputModel certificateEditInputModel)
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


        public Task<CertificateDetailsViewModel> CreateAsync(CertificateCreateInputModel certificatesCreateInputModel)
        {
            throw new System.NotImplementedException();
        }

        public Task<CertificateDetailsViewModel> DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string username)
        {
            var certificates = await this.certificatesRepository.All()
                .Where(x => x.IssuedTo == username)
                .To<CertificatesAllViewModel>()
                .OrderBy(x => x.Activity)
                .ThenByDescending(x => x.IssuedOn)
                .ToArrayAsync();

            return certificates;
        }

        public Task<CertificateDetailsViewModel> GetDetailsAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public bool CertificateExists(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
