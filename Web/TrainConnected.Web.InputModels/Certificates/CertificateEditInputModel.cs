namespace TrainConnected.Web.InputModels.Certificates
{
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class CertificateEditInputModel : IMapFrom<Certificate>
    {
        public string Id { get; set; }
    }
}
