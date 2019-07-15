namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.Certificates;

    public interface ICertificatesService
    {
        Task<CertificateDetailsViewModel> GetDetailsAsync(string id);

        Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string username);

        Task<CertificateDetailsViewModel> CreateAsync(CertificateCreateInputModel certificatesCreateInputModel);

        Task<CertificateDetailsViewModel> UpdateAsync(CertificateEditInputModel certificateEditInputModel);

        Task<CertificateDetailsViewModel> DeleteAsync(string id);

        bool CertificateExists(string id);
    }
}
