using MediatR;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using RobicServer.Models;
using RobicServer.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobicServer.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly IMediator _mediator;

        public PredictionService(IMediator mediator)
        {
            _mlContext = new MLContext();
            _mediator = mediator;
        }

        public async Task<object> PredictNetValue(string definitionId)
        {
            IEnumerable<Exercise> exercises = await _mediator.Send(new GetExercisesByDefinition()
            {
                DefinitionId = definitionId
            });
            // TOOD: Move into automapper profile
            var inputs = exercises.Select( e => new PredictModelInput()
            {
                ExerciseId = e.Id,
                Date = e.Date,
                NetValue = (float)e.NetValue,
                IsTrainingInput = e.Date.Year < 2021 ? 1 : 0
            });

            // TODO improve this exception handling
            // basically, we don't want to proceed if there is no data to predict or evaluate from  
            var trainingSetCount = inputs.Where(i => i.IsTrainingInput == 1).Count();
            var evalSetCount = inputs.Where(i => i.IsTrainingInput == 0).Count();
            if(trainingSetCount == 0 || evalSetCount == 0)
            {
                return null;
            }
            Console.WriteLine("---------------------");
            Console.WriteLine($"Training Data: {trainingSetCount}, Eval data: {evalSetCount}");
            Console.WriteLine("---------------------");


            IDataView dataView = _mlContext.Data.LoadFromEnumerable(inputs);
            IDataView trainingData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", upperBound: 1);
            IDataView evaluationData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", lowerBound: 1);

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedNetValue",
                inputColumnName: "NetValue",
                windowSize: 2, // analyzed on a 2 day basis
                seriesLength: 7, // weekly intervals
                trainSize: trainingSetCount,
                horizon: 7, // forecast 7 periods into the future
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundNetValue",
                confidenceUpperBoundColumn: "UpperBoundNetValue");

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(trainingData);

            return Evaluate(evaluationData, forecaster);
        }

        object Evaluate(IDataView testData, ITransformer model)
        {
            IDataView predictions = model.Transform(testData);
            
            IEnumerable<float> actual = _mlContext.Data.CreateEnumerable<PredictModelInput>(testData, true)
                .Select(observed => observed.NetValue);

            IEnumerable<float> forecast = _mlContext.Data.CreateEnumerable<PredictModelOutput>(predictions, true)
                .Select(prediction => prediction.ForecastedNetValue[0]);

            // Calculate the error (difference between actual and forecast)
            Console.WriteLine("Actual vs Forecast");
            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => {
                Console.WriteLine($"actualValue: {actualValue}, forecastValue: {forecastValue}");
                return actualValue - forecastValue;
            });

            var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error
            
            Console.WriteLine("---------------------");
            Console.WriteLine("Evaluation Metrics");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
            Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");

            return new
            {
                MAE = MAE,
                RMSE = RMSE
            };
        }
    }
}
