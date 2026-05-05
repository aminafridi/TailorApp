using System.Threading.Tasks;
using TailorApp.Models;

namespace TailorApp.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByCredentialsAsync(string loginName, string password);
    }
}
