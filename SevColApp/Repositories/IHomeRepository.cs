using SevColApp.Models;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public interface IHomeRepository
    {
        Task<int> AddUserIfHeDoesNotExits(User user);
        Task<User> FindUserById(int id);
        bool IsPasswordCorrect(string password, int userId);
    }
}
