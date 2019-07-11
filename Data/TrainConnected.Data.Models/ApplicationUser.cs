// ReSharper disable VirtualMemberCallInConstructor
namespace TrainConnected.Data.Models
{
    using System;
    using System.Collections.Generic;

    using TrainConnected.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity, IApplicationUser
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.Workouts = new HashSet<ApplicationUsersWorkouts>();
            this.Bookings = new HashSet<Booking>();
            this.Achievements = new HashSet<Achievement>();
            this.Certificates = new HashSet<Certificate>();
            this.Withdrawals = new HashSet<Withdrawal>();
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

        public string FullName { get; set; }

        public decimal Balance { get; set; }

        public ICollection<ApplicationUsersWorkouts> Workouts { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public ICollection<Achievement> Achievements { get; set; }

        public ICollection<Certificate> Certificates { get; set; }

        public ICollection<Withdrawal> Withdrawals { get; set; }
    }
}
