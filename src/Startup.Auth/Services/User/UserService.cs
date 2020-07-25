using Startup.Auth.Entities;
using MongoDB.Driver;
using Startup.Auth.Models;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;

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

            var passwordItems = ProducePasswordHash(password);

            user.PasswordHash = passwordItems.Item1;
            user.PasswordSalt = passwordItems.Item2;

            await _users.InsertOneAsync(user);
        }

        public async Task<Guid> Login(string email, string password)
        {
            User user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

            if (user != null)
            {
                bool isVerified = VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);

                if (isVerified)
                {
                    return user.Id;
                }
            }

            return Guid.Empty;
        }

        private (byte[], byte[]) ProducePasswordHash(string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return (passwordHash, passwordSalt);
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}