namespace TrainConnected.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;

    public class TrainConnectedDbContext : IdentityDbContext<TrainConnectedUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(TrainConnectedDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public TrainConnectedDbContext(DbContextOptions<TrainConnectedDbContext> options)
            : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Achievement> Achievements { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Withdrawal> Withdrawals { get; set; }

        public DbSet<Workout> Workouts { get; set; }

        public DbSet<WorkoutActivity> WorkoutActivities { get; set; }

        public DbSet<TrainConnectedUsersWorkouts> TrainConnectedUsersWorkouts { get; set; }

        public DbSet<TrainConnectedUsersBuddies> TrainConnectedUsersBuddies { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<WorkoutsPaymentMethods> WorkoutsPaymentMethods { get; set; }

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            ConfigureUserIdentityRelations(builder);

            EntityIndexesConfiguration.Configure(builder);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void ConfigureUserIdentityRelations(ModelBuilder builder)
        {
            builder.Entity<TrainConnectedUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainConnectedUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainConnectedUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(u => u.TrainConnectedUser)
                .WithMany(b => b.Bookings);

            builder.Entity<TrainConnectedUsersWorkouts>()
                .HasKey(uw => new { uw.TrainConnectedUserId, uw.WorkoutId });

            builder.Entity<TrainConnectedUsersWorkouts>()
                .HasOne(uw => uw.TrainConnectedUser)
                .WithMany(w => w.Workouts)
                .HasForeignKey(uw => uw.TrainConnectedUserId);

            builder.Entity<TrainConnectedUsersWorkouts>()
                .HasOne(uw => uw.Workout)
                .WithMany(u => u.Users)
                .HasForeignKey(uw => uw.WorkoutId);

            builder.Entity<TrainConnectedUsersBuddies>()
                .HasKey(ub => new { ub.TrainConnectedUserId, ub.TrainConnectedBuddyId });

            builder.Entity<TrainConnectedUsersBuddies>()
                .HasOne(ub => ub.TrainConnectedUser)
                .WithMany(b => b.Buddies)
                .HasForeignKey(ub => ub.TrainConnectedUserId);

            builder.Entity<WorkoutsPaymentMethods>()
                .HasKey(wp => new { wp.WorkoutId, wp.PaymentMethodId });

            builder.Entity<WorkoutsPaymentMethods>()
                .HasOne(wp => wp.Workout)
                .WithMany(p => p.PaymentMethods)
                .HasForeignKey(w => w.WorkoutId);

            builder.Entity<Withdrawal>()
                .HasOne(u => u.TrainConnectedUser)
                .WithMany(w => w.Withdrawals)
                .HasForeignKey(u => u.TrainConnectedUserId);
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
