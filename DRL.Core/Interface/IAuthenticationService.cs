using DRL.Library;

namespace DRL.Core.Interface
{
    public interface IAuthenticationService
    {
        ActionStatus Authenticate(string username, string password, string domain);
    }
}
