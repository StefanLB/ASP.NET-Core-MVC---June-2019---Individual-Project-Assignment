namespace TrainConnected.Web.ViewModels.Certificates
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class CertificateDetailsViewModel : IMapFrom<Certificate>
    {
        public string Id { get; set; }

        public string ActivityId { get; set; }

        [Display(Name = "Activity")]
        public string ActivityName { get; set; }

        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; }

        [Display(Name = "Issued On")]
        public DateTime IssuedOn { get; set; }

        [Display(Name = "Expires On")]
        public DateTime? ExpiresOn { get; set; }

        public string Description { get; set; }
    }
}
