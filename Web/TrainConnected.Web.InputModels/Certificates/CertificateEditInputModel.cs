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

        public string AcitvityId { get; set; }

        [Required]
        [Display(Name = ModelConstants.Certificate.ActivityNameDisplay)]
        public string ActivityName { get; set; }

        [Required]
        [StringLength(ModelConstants.Certificate.IssuedByMaxLength, MinimumLength = ModelConstants.Certificate.IsuedByMinLength, ErrorMessage = ModelConstants.Certificate.IssuedByLengthError)]
        public string IssuedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [LessThanOrEqual(nameof(DateTimeUtc), ErrorMessage = ModelConstants.Certificate.IssuedOnError)]
        public DateTime IssuedOn { get; set; }

        [DataType(DataType.Date)]
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
