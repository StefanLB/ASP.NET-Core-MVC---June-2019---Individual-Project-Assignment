namespace TrainConnected.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.Certificates;

    public class CertificatesService : ICertificatesService
    {
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

        public bool CertificateExists(string id)
        {
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

        public Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string username)
        {
            throw new System.NotImplementedException();
        }

        public Task<CertificateDetailsViewModel> GetDetailsAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
