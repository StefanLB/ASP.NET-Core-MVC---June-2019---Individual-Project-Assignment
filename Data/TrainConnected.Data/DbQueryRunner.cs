namespace TrainConnected.Data
{
    using System;
    using System.Threading.Tasks;

    using TrainConnected.Data.Common;

    using Microsoft.EntityFrameworkCore;

    public class DbQueryRunner : IDbQueryRunner
    {
        public DbQueryRunner(TrainConnectedDbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TrainConnectedDbContext Context { get; set; }

        public Task RunQueryAsync(string query, params object[] parameters)
        {
            return this.Context.Database.ExecuteSqlCommandAsync(query, parameters);
        }

        public void Dispose()
        {
            this.Context?.Dispose();
        }
    }
}
