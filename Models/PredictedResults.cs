using System.Collections.Generic;

namespace RobicServer.Models
{
    public class PredictedResults
    {
        public int TrainingSetCount { get; set; }
        public int EvaluationSetCount { get; set; }
        public float MeanAbsoluteError { get; set; }
        public double RootMeanSquaredError { get; set; }
        public IEnumerable<float> ActualResults { get; set; }
        public IEnumerable<float> ForecastedResults { get; set; }
        public PredictModelOutput PredictionResults { get; set; }
    }
}
