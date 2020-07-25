using System;
using System.Threading.Tasks;

namespace Startup.Auth.Services
{
    public interface IUserService
    {
        Task Register(string email, string password);
        Task<Guid> Login(string email, string password);
    }
}