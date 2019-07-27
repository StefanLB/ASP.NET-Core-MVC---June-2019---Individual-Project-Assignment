namespace TrainConnected.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrainConnected.Web.ViewModels.Users;

    public interface IUsersService
    {
        Task<IEnumerable<UsersAllViewModel>> GetAllAsync(string adminId);

        Task UnlockUserAsync(string id);

        Task LockUserAsync(string id);

        Task<UserDetailsViewModel> GetUserDetailsAsync(string id);

        Task<IEnumerable<string>> GetAllRolesAsync();
    }
}
