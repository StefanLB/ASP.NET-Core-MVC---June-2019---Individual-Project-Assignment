namespace TrainConnected.Web.ViewModels.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class UserDetailsViewModel : IMapFrom<TrainConnectedUser>
    {
        public UserDetailsViewModel()
        {
            this.RolesNames = new HashSet<string>();
        }

        [Display(Name = ModelConstants.User.UserIdNameDisplay)]
        public string Id { get; set; }

        [Display(Name = ModelConstants.User.UserNameDisplay)]
        public string UserName { get; set; }

        [Display(Name = ModelConstants.User.FirstNameDisplay)]
        public string FirstName { get; set; }

        [Display(Name = ModelConstants.User.LastNameDisplay)]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name = ModelConstants.User.PhoneNumberNameDisplay)]
        public string PhoneNumber { get; set; }

        [Display(Name = ModelConstants.User.RolesNameDisplay)]
        public ICollection<string> RolesNames { get; set; }
    }
}
