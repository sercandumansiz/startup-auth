using Startup.Auth.Entities;
using MongoDB.Driver;
using Startup.Auth.Models;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Startup.Auth.Provider;

namespace Startup.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserRefreshToken> _userRefreshTokens;
        private readonly IMongoCollection<InvalidToken> _invalidTokens;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtProvider _jwtProvider;


        public UserService(IStartupAuthDatabaseSettings settings, IPasswordHasher<User> passwordHasher, IJwtProvider jwtProvider)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
            _userRefreshTokens = database.GetCollection<UserRefreshToken>(settings.AuthsCollectionName);
            _invalidTokens = database.GetCollection<InvalidToken>(settings.InvalidTokensCollectionName);

            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<BaseModel<bool>> Register(string email, string password)
        {
            BaseModel<bool> result = new BaseModel<bool>();

            try
            {
                bool isRegistered = await _users.Find(u => u.Email == email).AnyAsync();

                if (isRegistered)
                {
                    result.HasError = true;
                    result.ErrorMessage = "This e-mail address is already registered.";

                    return result;
                }

                User user = new User();
                user.Email = email;
                user.CreatedAt = DateTime.Now;

                var passwordItems = ProducePasswordHash(password);

                user.PasswordHash = passwordItems.Item1;
                user.PasswordSalt = passwordItems.Item2;

                await _users.InsertOneAsync(user);

                result.Data = true;
            }
            catch
            {
                throw new SystemException("Something went wrong while registering user.");
            }

            return result;
        }

        public async Task<BaseModel<UserModel>> Login(string email, string password)
        {
            BaseModel<UserModel> result = new BaseModel<UserModel>();

            try
            {
                User user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

                if (user != null)
                {
                    bool isVerified = VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);

                    if (isVerified)
                    {
                        string refreshToken = _passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                        .Replace("+", string.Empty)
                        .Replace("=", string.Empty)
                        .Replace("/", string.Empty);

                        UserRefreshToken userRefreshToken = new UserRefreshToken()
                        {
                            UserId = user.Id,
                            RefreshToken = refreshToken
                        };

                        await _userRefreshTokens.InsertOneAsync(userRefreshToken);

                        result.Data = new UserModel
                        {
                            Id = user.Id,
                            Email = user.Email,
                            CreatedAt = user.CreatedAt,
                            Type = user.Type,
                            RefreshToken = refreshToken
                        };

                        string token = _jwtProvider.GenerateToken(result.Data);

                        result.Data.Token = token;
                    }
                }
            }
            catch
            {
                throw new SystemException("Something went wrong while verifiying user.");
            }

            return result;
        }

        public async Task<BaseModel<UserModel>> RefreshAccessToken(string refreshToken)
        {
            BaseModel<UserModel> result = new BaseModel<UserModel>();

            try
            {
                UserRefreshToken userRefreshToken = await _userRefreshTokens.Find(t => t.RefreshToken == refreshToken).FirstOrDefaultAsync();

                if (userRefreshToken == null || userRefreshToken.IsRevoked)
                {
                    result.HasError = true;
                    result.ErrorMessage = "Refresh token not found or revoked.";

                    return result;
                }

                User user = await _users.Find(u => u.Id == userRefreshToken.UserId).FirstOrDefaultAsync();

                if (user == null)
                {
                    result.HasError = true;
                    result.ErrorMessage = "User not found.";

                    return result;
                }

                result.Data = new UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    Type = user.Type,
                    RefreshToken = refreshToken
                };

                string token = _jwtProvider.GenerateToken(result.Data);

                result.Data.Token = token;
            }
            catch
            {
                throw new SystemException("Something went wrong while refreshing access token.");
            }

            return result;
        }

        public async Task<BaseModel<bool>> Logout(Guid userId, string token, string refreshToken)
        {
            BaseModel<bool> result = new BaseModel<bool>();

            try
            {
                var deleteResult = await _userRefreshTokens.DeleteOneAsync(u => u.RefreshToken == refreshToken);

                if (deleteResult.IsAcknowledged)
                {
                    InvalidToken invalidToken = new InvalidToken()
                    {
                        Token = token
                    };

                    await _invalidTokens.InsertOneAsync(invalidToken);

                    result.Data = true;
                }
                else
                {
                    throw new SystemException("Something went wrong while deleting access token.");
                }
            }
            catch (System.Exception)
            {
                throw new SystemException("Something went wrong while deleting access token.");
            }

            return result;
        }

        public async Task<BaseModel<bool>> Introspect(string token)
        {
            BaseModel<bool> result = new BaseModel<bool>();

            try
            {
                bool isInvalidToken = await _invalidTokens.Find(t => t.Token == token).AnyAsync();

                result.Data = isInvalidToken;
            }
            catch
            {
                throw new SystemException("Something went wrong while introspecting access token.");
            }

            return result;
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