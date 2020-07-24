using System.Threading.Tasks;

namespace Startup.Auth.Services
{
    public interface IUserService
    {
        Task Register(string email, string password);
    }
}