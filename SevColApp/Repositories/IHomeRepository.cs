using SevColApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public interface IHomeRepository
    {
        int AddUserIfHeDoesNotExits(User user);
        User FindUserById(int id);
        bool IsPasswordCorrect(string password, int userId);
        bool LoginIsCorrect(User user);
        int FindUserIdByLoginName(string name);
        List<User> GetAllOtherUsers(int id);
        void DeleteUserById(int id);
    }
}
