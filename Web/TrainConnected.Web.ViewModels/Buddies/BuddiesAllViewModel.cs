using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TrainConnected.Data.Models;
using TrainConnected.Services.Mapping;

namespace TrainConnected.Web.ViewModels.Buddies
{
    public class BuddiesAllViewModel : IMapFrom<TrainConnectedUser>
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name ="Workouts")]
        public int WorkoutsCount { get; set; }

        [Display(Name ="Achievements")]
        public int AchievementsCount { get; set; }
    }
}
