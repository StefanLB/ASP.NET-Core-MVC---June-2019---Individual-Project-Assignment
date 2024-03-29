﻿// ReSharper disable VirtualMemberCallInConstructor
namespace TrainConnected.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.AspNetCore.Identity;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    [Table("AspNetUsers")]
    public class TrainConnectedUser : IdentityUser, IAuditInfo, IDeletableEntity, ITrainConnectedUser
    {
        public TrainConnectedUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.Workouts = new HashSet<TrainConnectedUsersWorkouts>();
            this.Bookings = new HashSet<Booking>();
            this.Achievements = new HashSet<Achievement>();
            this.Certificates = new HashSet<Certificate>();
            this.Withdrawals = new HashSet<Withdrawal>();
            this.Buddies = new HashSet<TrainConnectedUsersBuddies>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        // Application properties
        public string Password { get; set; }

        [Required]
        [RegularExpression(ModelConstants.NameRegex, ErrorMessage = ModelConstants.NameRegexError)]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(ModelConstants.NameRegex, ErrorMessage = ModelConstants.NameRegexError)]
        public string LastName { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public string ProfilePicture { get; set; }

        public ICollection<TrainConnectedUsersWorkouts> Workouts { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public ICollection<Achievement> Achievements { get; set; }

        public ICollection<Certificate> Certificates { get; set; }

        public ICollection<Withdrawal> Withdrawals { get; set; }

        public ICollection<TrainConnectedUsersBuddies> Buddies { get; set; }
    }
}
