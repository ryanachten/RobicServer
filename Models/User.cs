using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RobicServer.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        // [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; }

        // [BsonElement("passwordSalt")]
        public byte[] PasswordSalt { get; set; }

        [BsonElement("exercises")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ICollection<string> Exercises { get; set; }
    }
}