﻿namespace TrainConnected.Web.ViewModels.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class UsersAllViewModel : IMapFrom<TrainConnectedUser>
    {
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

        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(Name = ModelConstants.User.LockedOutNameDisplay)]
        public DateTime? LockoutEndDateTime { get; set; }
    }
}
