namespace TrainConnected.Web.ViewModels.Buddies
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BuddyDetailsViewModel : IMapFrom<TrainConnectedUser>
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Joined On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Workouts")]
        public int WorkoutsCount { get; set; }

        [Display(Name = "Achievements")]
        public int AchievementsCount { get; set; }

        [Display(Name ="Buddies Since")]
        public DateTime AddedOn { get; set; }
    }
}
