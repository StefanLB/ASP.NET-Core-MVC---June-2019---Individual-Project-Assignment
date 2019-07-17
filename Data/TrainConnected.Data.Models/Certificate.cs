﻿namespace TrainConnected.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;

    public class Certificate : BaseDeletableModel<string>, ICertificate
    {
        public string ActivityId { get; set; }

        [Required]
        public WorkoutActivity Activity { get; set; }

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

        public string TrainConnectedUserId { get; set; }
        public TrainConnectedUser TrainConnectedUser { get; set; }

    }
}
