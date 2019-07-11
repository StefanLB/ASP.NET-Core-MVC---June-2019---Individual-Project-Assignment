namespace TrainConnected.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public interface ISeeder
    {
        Task SeedAsync(TrainConnectedDbContext dbContext, IServiceProvider serviceProvider);
    }
}
