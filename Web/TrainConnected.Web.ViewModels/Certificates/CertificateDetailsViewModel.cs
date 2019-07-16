namespace TrainConnected.Web.ViewModels.Certificates
{
    using System;

    using TrainConnected.Data.Models;
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Services.Mapping;

    public class CertificateDetailsViewModel : IMapFrom<Certificate>
    {
        public string Id { get; set; }

        public WorkoutActivity Activity { get; set; }

        public string IssuedBy { get; set; }

        public string IssuedTo { get; set; }

        public DateTime IssuedOn { get; set; }

        public DateTime? ExpiresOn { get; set; }

        public string Description { get; set; }
    }
}
