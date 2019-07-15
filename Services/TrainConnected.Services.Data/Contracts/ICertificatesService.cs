namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TrainConnected.Web.ViewModels.Certificates;

    public interface ICertificatesService
    {
        Task<CertificateDetailsViewModel> GetDetailsAsync(string id);

        Task<IEnumerable<CertificatesAllViewModel>> GetAllAsync(string username);
    }
}
