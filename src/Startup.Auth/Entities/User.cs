using System;
using MongoDB.Bson.Serialization.Attributes;
using Startup.Auth.Constants;

namespace Startup.Auth.Entities
{
    public class User
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("PasswordHash")]
        public byte[] PasswordHash { get; set; }
        [BsonElement("PasswordSalt")]
        public byte[] PasswordSalt { get; set; }
        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions]
        public DateTime CreatedAt { get; set; }
        [BsonElement("LoginType")]
        public LoginType LoginType { get; set; } = LoginType.Internal;
        [BsonElement("Role")]
        public string Type { get; set; } = Role.Observer;
    }

    public enum LoginType
    {
        Internal = 1,
        Twitter = 2
    }
}