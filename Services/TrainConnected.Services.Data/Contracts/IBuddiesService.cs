using System.Collections.Generic;
using System.Threading.Tasks;
using TrainConnected.Web.ViewModels.Buddies;

namespace TrainConnected.Services.Data.Contracts
{
    public interface IBuddiesService
    {
        Task AddAsync(string id, string userId);
        Task<BuddyDetailsViewModel> GetDetailsAsync(string id, string userId);
        Task RemoveAsync(string id, string userId);
        Task<IEnumerable<BuddiesAllViewModel>> GetAllAsync(string userId);
        Task<IEnumerable<BuddiesAllViewModel>> FindAllAsync(string userId);
    }
}
