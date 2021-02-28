using SevColApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public interface IHomeRepository
    {
        Task<int> AddUserIfHeDoesNotExits(User user);
        Task<User> FindUserById(int id);
        bool IsPasswordCorrect(string password, int userId);
        bool LoginIsCorrect(User user);
        int FindUserIdByLoginName(string name);
        Task<List<User>> GetAllOtherUsers(int id);
    }
}
