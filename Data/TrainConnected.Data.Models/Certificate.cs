namespace TrainConnected.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class Certificate : BaseDeletableModel<string>, ICertificate
    {
        [Required]
        public string ActivityId { get; set; }

        public WorkoutActivity Activity { get; set; }

        [Required]
        [StringLength(ModelConstants.Certificate.IssuedByMaxLength, MinimumLength = ModelConstants.Certificate.IsuedByMinLength, ErrorMessage = ModelConstants.Certificate.IssuedByLengthError)]
        public string IssuedBy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime IssuedOn { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiresOn { get; set; }

        [Required]
        [StringLength(ModelConstants.Certificate.DescriptionMaxLength, MinimumLength = ModelConstants.Certificate.DescriptionMinLength, ErrorMessage = ModelConstants.DescriptionLengthError)]
        public string Description { get; set; }

        [Required]
        public string TrainConnectedUserId { get; set; }
        public virtual TrainConnectedUser TrainConnectedUser { get; set; }
    }
}
