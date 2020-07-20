using System;
using System.Collections.Generic;
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

        [BsonElement("timeTaken")]
        public DateTime TimeTaken { get; set; }

        // TODO: this should be calculated on the API, not stored in the DB (isn't )
        // [BsonRepresentation(BsonType.Double)]
        // public double netValue { get; set; }

        [BsonElement("sets")]
        public ICollection<Set> sets { get; set; }
    }
}