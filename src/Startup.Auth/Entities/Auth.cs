using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Startup.Auth.Entities
{
    public class UserRefreshToken
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("UserId")]
        public Guid UserId { get; set; }
        [BsonElement("RefreshToken")]
        public string RefreshToken { get; set; }
        [BsonElement("IsRevoked")]
        public bool IsRevoked { get; set; }
    }
}