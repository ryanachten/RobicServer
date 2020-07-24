using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RobicServer.Helpers;

namespace RobicServer.Models
{
    [BsonCollection("users")]
    [BsonIgnoreExtraElements]
    public class Exercise : Document
    {
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
                if (this.sets == null)
                    return total;

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
            set { netValue = value; }
        }

    }
}