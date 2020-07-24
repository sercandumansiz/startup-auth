using Startup.Auth.Entities;
using MongoDB.Driver;
using Startup.Auth.Models;
using System.Threading.Tasks;
using System;
using Startup.Auth.Helpers;

namespace Startup.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IStartupAuthDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task Register(string email, string password)
        {
            User user = new User();

            user.Email = email;
            user.CreatedAt = DateTime.Now;
            user.PasswordHash = SecurityHelper.ProducePasswordHash(password).Item1;
            user.PasswordSalt = SecurityHelper.ProducePasswordHash(password).Item2;

            await _users.InsertOneAsync(user);
        }
    }
}