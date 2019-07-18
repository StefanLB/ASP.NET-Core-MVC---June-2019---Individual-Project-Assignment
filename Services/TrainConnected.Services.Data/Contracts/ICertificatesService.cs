namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.Certificates;

    public interface ICertificatesService
    {
        Task<CertificateDetailsViewModel> GetDetailsAsync(string id);

        Task<CertificateEditInputModel> GetEditDetailsAsync(string id);

        Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string username);

        Task<CertificateDetailsViewModel> CreateAsync(CertificateCreateInputModel certificatesCreateInputModel, string username);

        Task UpdateAsync(CertificateEditInputModel certificateEditInputModel);

        Task DeleteAsync(string id);
    }
}
