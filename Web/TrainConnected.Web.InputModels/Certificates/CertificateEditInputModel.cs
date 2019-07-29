﻿namespace TrainConnected.Web.InputModels.Certificates
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Common.Attributes;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class CertificateEditInputModel : IMapFrom<Certificate>
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string AcitvityId { get; set; }

        [Required]
        [Display(Name = ModelConstants.Certificate.ActivityNameDisplay)]
        public string ActivityName { get; set; }

        [Required]
        [Display(Name = ModelConstants.Certificate.IssuedByNameDisplay)]
        [StringLength(ModelConstants.Certificate.IssuedByMaxLength, MinimumLength = ModelConstants.Certificate.IsuedByMinLength, ErrorMessage = ModelConstants.Certificate.IssuedByLengthError)]
        public string IssuedBy { get; set; }

        [Required]
        [Display(Name = ModelConstants.Certificate.IssuedOnNameDisplay)]
        [DataType(DataType.Date)]
        [LessThanOrEqual(nameof(DateTimeUtc), ErrorMessage = ModelConstants.Certificate.IssuedOnError)]
        public DateTime IssuedOn { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = ModelConstants.Certificate.ExpiresOnNameDisplay)]
        [GreaterThanOrEqualNullableDateAttribute(nameof(DateTimeUtc), ErrorMessage = ModelConstants.Certificate.ExpiresOnError)]
        public DateTime? ExpiresOn { get; set; }

        [Required]
        [StringLength(ModelConstants.Certificate.DescriptionMaxLength, MinimumLength = ModelConstants.Certificate.DescriptionMinLength, ErrorMessage = ModelConstants.DescriptionLengthError)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTimeUtc
        {
            get { return DateTime.UtcNow.Date; }
        }
    }
}
