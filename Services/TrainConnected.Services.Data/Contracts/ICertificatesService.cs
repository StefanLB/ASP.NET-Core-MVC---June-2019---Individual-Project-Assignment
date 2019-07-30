namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.InputModels.Certificates;
    using TrainConnected.Web.ViewModels.Certificates;

    public interface ICertificatesService
    {
        Task<CertificateDetailsViewModel> GetDetailsAsync(string id, string userId);

        Task<CertificateEditInputModel> GetEditDetailsAsync(string id, string userId);

        Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string userId);

        Task<CertificateDetailsViewModel> CreateAsync(CertificateCreateInputModel certificatesCreateInputModel, string userId);

        Task UpdateAsync(CertificateEditInputModel certificateEditInputModel, string userId);

        Task DeleteAsync(string id, string userId);
    }
}
