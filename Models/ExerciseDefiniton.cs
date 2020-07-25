using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RobicServer.Helpers;

namespace RobicServer.Models
{
    [BsonCollection("exercisedefinitions")]
    [BsonIgnoreExtraElements]
    public class ExerciseDefiniton : Document
    {
        [BsonElement("title")]
        public string Title { get; set; }

        // TODO: should be enum or something
        [BsonElement("unit")]
        public string Unit { get; set; }

        // TODO: should be enum or something
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("user")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string User { get; set; }

        [BsonElement("history")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ICollection<string> History { get; set; }

        [BsonElement("childExercises")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ICollection<string> ChildExercises { get; set; }

        [BsonElement("primaryMuscleGroup")]
        [BsonRepresentation(BsonType.String)]
        public ICollection<string> PrimaryMuscleGroup { get; set; }
    }
}