using System.Threading.Tasks;

namespace TrainConnected.Services.Data.Contracts
{
    public interface IBuddiesService
    {
        Task AddAsync(string id, string userId);
    }
}
