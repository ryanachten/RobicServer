namespace RobicServer.Models
{
    public class PredictModelOutput
    {
        public double[] ForecastedNetValue { get; set; }
        public double[] LowerBoundNetValue { get; set; }
        public double[] UpperBoundNetValue { get; set; }

    }
}
