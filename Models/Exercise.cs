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

        [BsonElement("Name")]
        public string ExerciseName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string definition { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string session { get; set; }

        // TODO: this should be calculated on the API, not stored in the DB
        [BsonRepresentation(BsonType.Double)]
        public double netValue { get; set; }

        // TODO: sets
        public string sets { get; set; }
    }
}