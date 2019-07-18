namespace TrainConnected.Web.InputModels.Certificates
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Data.Models.Enums;
    using TrainConnected.Services.Mapping;

    public class CertificateEditInputModel : IMapFrom<Certificate>
    {
        [Required]
        public string Id { get; set; }

        public string AcitvityId { get; set; }

        [Required]
        [Display(Name = "Activity")]
        public string ActivityName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string IssuedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime IssuedOn { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiresOn { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Field cannot contain more than 200 characters")]
        public string Description { get; set; }
    }
}
