namespace TrainConnected.Data.Models
{
    using System;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Enums;

    public class Certificate : BaseDeletableModel<string>, ICertificate
    {
        public WorkoutActivity Activity { get; set; }

        public string IssuedBy { get; set; }

        public string IssuedTo { get; set; }

        public DateTime IssuedOn { get; set; }

        public DateTime? ExpiresOn { get; set; }

        public string Description { get; set; }
    }
}
