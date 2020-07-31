using System;
using System.Threading.Tasks;
using Startup.Auth.Entities;
using Startup.Auth.Models;

namespace Startup.Auth.Services
{
    public interface IUserService
    {
        Task<BaseModel<bool>> Register(string email, string password);
        Task<BaseModel<UserModel>> Login(string email, string password);
        Task<BaseModel<UserModel>> RefreshAccessToken(string refreshToken);
        Task<BaseModel<bool>> Logout(Guid userId, string token, string refreshToken);
        Task<BaseModel<bool>> Introspect(string token);
    }
}