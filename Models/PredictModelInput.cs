using Microsoft.ML.Data;
using System;

namespace RobicServer.Models
{
    public class PredictModelInput
    {
        public string ExerciseId { get; set; }
        public DateTime Date { get; set; }
        public double NetValue { get; set; }
        public int IsTrainingInput { get; set; }

    }
}
