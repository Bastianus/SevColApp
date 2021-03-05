using SevColApp.Models;

namespace SevColApp.Repositories
{
    public interface IGamemasterRepository
    {
        User ChangeUserPassword(int userId, string newPassword);
    }
}
