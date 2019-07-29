namespace TrainConnected.Web.ViewModels.Certificates
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class CertificatesAllViewModel : IMapFrom<Certificate>
    {
        public string Id { get; set; }

        public string Activityid { get; set; }

        [Display(Name = ModelConstants.Certificate.ActivityNameDisplay)]
        public string ActivityName { get; set; }

        [Display(Name = ModelConstants.Certificate.IssuedByNameDisplay)]
        public string IssuedBy { get; set; }

        [Display(Name = ModelConstants.Certificate.IssuedOnNameDisplay)]
        public DateTime IssuedOn { get ; set; }

        [Display(Name = ModelConstants.Certificate.ExpiresOnNameDisplay)]
        public DateTime? ExpiresOn { get; set; }

        public string Description { get; set; }
    }
}
