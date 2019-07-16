namespace TrainConnected.Web.ViewModels.Certificates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using TrainConnected.Data.Models;
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Services.Mapping;

    public class CertificatesAllViewModel : IMapFrom<Certificate>
    {
        public string Id { get; set; }

        public WorkoutActivity Activity { get; set; }

        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; }

        [Display(Name = "Issued On")]
        public DateTime IssuedOn { get ; set; }

        [Display(Name = "Expires On")]
        public DateTime? ExpiresOn { get; set; }

        public string Description { get; set; }
    }
}
