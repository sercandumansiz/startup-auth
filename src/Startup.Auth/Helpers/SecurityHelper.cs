using System.Text;
using System.Security.Cryptography;

namespace Startup.Auth.Helpers
{
    public static class SecurityHelper
    {
        public static (byte[], byte[]) ProducePasswordHash(string password)
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
    }
}