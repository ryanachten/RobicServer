using Microsoft.ML.Data;
using System;

namespace RobicServer.Models
{
    public class PredictModelInput
    {
        public string ExerciseId { get; set; }
        public DateTime Date { get; set; }
        [ColumnName("NetValue")]
        public double NetValue { get; set; }
        [ColumnName("IsTrainingInput")]
        public double IsTrainingInput { get; set; }

    }
}
