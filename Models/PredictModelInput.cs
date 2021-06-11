using Microsoft.ML.Data;
using System;

namespace RobicServer.Models
{
    public class PredictModelInput
    {
        public string ExerciseId { get; set; }
        public DateTime Date { get; set; }
        [ColumnName("NetValue")]
        public float NetValue { get; set; }
        [ColumnName("IsTrainingInput")]
        public float IsTrainingInput { get; set; }

    }
}
