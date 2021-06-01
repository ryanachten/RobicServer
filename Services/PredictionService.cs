using MediatR;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using RobicServer.Models;
using RobicServer.Query;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public async void PredictNetValue(string definitionId)
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
                NetValue = (double)e.NetValue,
                IsTrainingInput = e.Date.Year < 2021 ? 1 : 0
            });

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(inputs);
            IDataView trainingData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", upperBound: 1);
            IDataView evaluationData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", lowerBound: 1);

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedNetValue",
                inputColumnName: "NetValue",
                windowSize: 7, // analyzed on a weekly basis
                seriesLength: 30, // monthly intervals
                trainSize: 365, // 365 data points for the first year
                horizon: 7, // forecast 7 periods into the future
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundNetValue",
                confidenceUpperBoundColumn: "UpperBoundNetValue");

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(trainingData);

            //Evaluate(evaluationData, forecaster);
        }

        void Evaluate(IDataView testData, ITransformer model)
        {
            IDataView predictions = model.Transform(testData);
            
            IEnumerable<double> actual = _mlContext.Data.CreateEnumerable<PredictModelInput>(testData, true)
                .Select(observed => observed.NetValue);

            IEnumerable<double> forecast = _mlContext.Data.CreateEnumerable<PredictModelOutput>(predictions, true)
                .Select(prediction => prediction.ForecastedNetValue[0]);

            // Calculate the error (difference between actual and forecast)
            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

            var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

            Console.WriteLine("Evaluation Metrics");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
            Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");
        }
    }
}
