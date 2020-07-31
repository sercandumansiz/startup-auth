using System;

namespace Startup.Auth.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
        public string RefreshToken { get; set; }
        public string Token { get; set; }
    }
}
