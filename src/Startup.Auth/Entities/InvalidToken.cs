using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Startup.Auth.Entities
{
    public class InvalidToken
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("Token")]
        public string Token { get; set; }
    }
}