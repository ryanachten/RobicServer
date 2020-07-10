using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RobicServer.Models
{
    public class Exercise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}