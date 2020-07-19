using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RobicServer.Models
{
    [BsonIgnoreExtraElements]
    public class Exercise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("definition")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Definition { get; set; }

        [BsonElement("session")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Session { get; set; }

        // TODO: this should be calculated on the API, not stored in the DB (isn't )
        // [BsonRepresentation(BsonType.Double)]
        // public double netValue { get; set; }

        // // TODO: sets
        // public string sets { get; set; }
    }
}