namespace TrainConnected.Web.ViewModels.Buddies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.ViewModels.Certificates;

    public class CoachDetailsViewModel : IMapFrom<TrainConnectedUser>
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.User.UserNameDisplay)]
        public string UserName { get; set; }

        [Display(Name = ModelConstants.User.FirstNameDisplay)]
        public string FirstName { get; set; }

        [Display(Name = ModelConstants.User.LastNameDisplay)]
        public string LastName { get; set; }

        [Display(Name = ModelConstants.User.JoinedOnNameDisplay)]
        public DateTime CreatedOn { get; set; }

        [Display(Name = ModelConstants.User.WorkoutsCoachedNameDisplay)]
        public int WorkoutsCoached { get; set; }

        public IEnumerable<CertificatesAllViewModel> Certificates { get; set; }
    }
}
