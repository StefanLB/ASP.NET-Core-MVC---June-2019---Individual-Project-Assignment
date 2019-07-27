namespace TrainConnected.Web.ViewModels.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class UserDetailsViewModel : IMapFrom<TrainConnectedUser>
    {
        public UserDetailsViewModel()
        {
            this.RolesNames = new HashSet<string>();
        }

        [Display(Name = "User Id")]
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name ="Assigned Roles")]
        public ICollection<string> RolesNames { get; set; }
    }
}
