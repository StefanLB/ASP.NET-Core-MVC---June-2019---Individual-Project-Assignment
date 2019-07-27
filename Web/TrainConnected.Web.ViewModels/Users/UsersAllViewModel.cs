namespace TrainConnected.Web.ViewModels.Users
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class UsersAllViewModel : IMapFrom<TrainConnectedUser>
    {
        [Display(Name ="User Id")]
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name ="Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name ="Locked Until")]
        public DateTime? LockoutEnabled { get; set; }
    }
}
