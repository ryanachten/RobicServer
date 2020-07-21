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

        [BsonElement("timeTaken")]
        public DateTime TimeTaken { get; set; }

        [BsonElement("sets")]
        public ICollection<Set> sets { get; set; }

        private double? netValue;
        public double? NetValue
        {
            get
            {
                double? total = null;
                foreach (Set set in this.sets)
                {
                    if (set.Reps.HasValue && set.Value.HasValue)
                    {
                        if (total == null)
                        {
                            total = 0.0;
                        }
                        total += ((int)set.Reps * (double)set.Value);
                    }

                }
                return total;
            }
            set { NetValue = value; }
        }

    }
}