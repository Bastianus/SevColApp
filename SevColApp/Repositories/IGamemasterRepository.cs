using SevColApp.Models;

namespace SevColApp.Repositories
{
    public interface IGamemasterRepository
    {
        UserPasswordChange ChangeUserPassword(UserPasswordChange input);
        AccountPasswordChange ChangeBankAccountPassword(AccountPasswordChange input);
    }
}
