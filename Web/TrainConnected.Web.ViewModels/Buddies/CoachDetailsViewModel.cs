namespace TrainConnected.Web.ViewModels.Buddies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.ViewModels.Certificates;

    public class CoachDetailsViewModel : IMapFrom<TrainConnectedUser>
    {
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Joined On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name ="Workouts Coached")]
        public int WorkoutsCoached { get; set; }

        public IEnumerable<CertificatesAllViewModel> Certificates { get; set; }
    }
}
