namespace TrainConnected.Web.ViewModels.Buddies
{
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class BuddiesAllViewModel : IMapFrom<TrainConnectedUser>
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.User.UserNameDisplay)]
        public string UserName { get; set; }

        [Display(Name = ModelConstants.User.FirstNameDisplay)]
        public string FirstName { get; set; }

        [Display(Name = ModelConstants.User.LastNameDisplay)]
        public string LastName { get; set; }

        [Display(Name = ModelConstants.User.WorkoutsCountNameDisplay)]
        public int WorkoutsCount { get; set; }

        [Display(Name = ModelConstants.User.AchievementsNameDisplay)]
        public int AchievementsCount { get; set; }
    }
}
