namespace RobicServer.Models
{
    public class PredictModelOutput
    {
        public float[] ForecastedNetValue { get; set; }
        public float[] LowerBoundNetValue { get; set; }
        public float[] UpperBoundNetValue { get; set; }

    }
}
