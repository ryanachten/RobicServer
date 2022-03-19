using MediatR;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Microsoft.ML.Data;
using RobicServer.Models;
using RobicServer.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobicServer.Services
{
    struct LoadedData {
        public IDataView TrainingData { get; set; }
        public int TrainingSetCount { get; set; }
        public IDataView EvaluationData { get; set; }
        public int EvalutationSetCount { get; set; }
    }


    public class PredictionService : IPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly IMediator _mediator;

        public PredictionService(IMediator mediator)
        {
            _mlContext = new MLContext(seed: 0);
            _mediator = mediator;
        }

        private async Task<LoadedData?> LoadData(string definitionId)
        {
            IEnumerable<Exercise> exercises = await _mediator.Send(new GetExercisesByDefinition()
            {
                DefinitionId = definitionId
            });
            // TOOD: Move into automapper profile
            var inputs = exercises.Select(e => new PredictModelInput()
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
            if (trainingSetCount == 0 || evalSetCount == 0)
            {
                return null;
            }

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(inputs);
            IDataView trainingData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", upperBound: 1);
            IDataView evaluationData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", lowerBound: 1);
            return new LoadedData()
            {
                TrainingData = trainingData,
                TrainingSetCount = trainingSetCount,
                EvaluationData = evaluationData,
                EvalutationSetCount = evalSetCount,
            };
        }

        public async Task RegressionNetValue(string definitionId)
        {
            var loadedData = await LoadData(definitionId);
            if (loadedData == null)
            {
                return;
            }
            var data = (LoadedData)loadedData;

            var pipeline = _mlContext
                .Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "NetValue")
                           .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "ExerciseIdEncoded", inputColumnName: "ExerciseId"))
                           .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DateEncoded", inputColumnName: "Date"))
                           .Append(_mlContext.Transforms.Concatenate("Features", "ExerciseIdEncoded", "DateEncoded"))
                           .Append(_mlContext.Regression.Trainers.FastTree());

            var model = pipeline.Fit(data.TrainingData);

            EvaluateRegression(data.TrainingData, model);
        }

        private void EvaluateRegression(IDataView testData, ITransformer model)
        {
            var predictions = model.Transform(testData);

            var metrics = _mlContext.Regression.Evaluate(predictions, "Label", "Score");
            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
        }

        public async Task<PredictedResults> ForecastNetValue(string definitionId)
        {
            var loadedData = await LoadData(definitionId);
            if(loadedData == null)
            {
                return null;
            }
            var data = (LoadedData)loadedData;
            

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedNetValue",
                inputColumnName: "NetValue",
                windowSize: 2, // analyzed on a 2 day basis
                seriesLength: 7, // weekly intervals
                trainSize: data.TrainingSetCount,
                horizon: 7, // forecast 7 periods into the future
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundNetValue",
                confidenceUpperBoundColumn: "UpperBoundNetValue");

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(data.TrainingData);

            var results = EvaluateForecast(data.EvaluationData, forecaster);

            var forecasterEngine = forecaster.CreateTimeSeriesEngine<PredictModelInput, PredictModelOutput>(_mlContext);
            var predictionResults = Forecast(forecasterEngine, data.EvaluationData, 7);

            results.EvaluationSetCount = data.EvalutationSetCount;
            results.TrainingSetCount = data.TrainingSetCount;
            results.PredictionResults = predictionResults;

            return results;
        }

        PredictedResults EvaluateForecast(IDataView testData, ITransformer model)
        {
            IDataView predictions = model.Transform(testData);
            
            IEnumerable<float> actual = _mlContext.Data.CreateEnumerable<PredictModelInput>(testData, true)
                .Select(observed => observed.NetValue);

            IEnumerable<float> forecast = _mlContext.Data.CreateEnumerable<PredictModelOutput>(predictions, true)
                .Select(prediction => prediction.ForecastedNetValue[0]);

            // Calculate the error (difference between actual and forecast)
            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

            var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

            return new PredictedResults
            {
                MeanAbsoluteError = MAE,
                RootMeanSquaredError = RMSE,
                ActualResults = actual,
                ForecastedResults = forecast,
            };
        }

        private PredictModelOutput Forecast(TimeSeriesPredictionEngine<PredictModelInput, PredictModelOutput> forecaster, IDataView testData, int horizon)
        {
           var forecast = forecaster.Predict();

            IEnumerable<string> forecastOutput = _mlContext.Data.CreateEnumerable<PredictModelInput>(testData, reuseRowObject: false)
            .Take(horizon)
            .Select((PredictModelInput exercise, int index) =>
            {
                // TODO: dates don't align with what we're expecitng here - investigate
                string date = exercise.Date.ToShortDateString();
                float actualNetValue = exercise.NetValue;
                float lowerEstimate = Math.Max(0, forecast.LowerBoundNetValue[index]);
                float estimate = forecast.ForecastedNetValue[index];
                float upperEstimate = forecast.UpperBoundNetValue[index];
                return $"Date: {date}\n" +
                $"Actual NetValue: {actualNetValue}\n" +
                $"Lower Estimate: {lowerEstimate}\n" +
                $"Forecast: {estimate}\n" +
                $"Upper Estimate: {upperEstimate}\n";
            });

            Console.WriteLine("Exercise Forecast");
            Console.WriteLine("---------------------");
            foreach (var prediction in forecastOutput)
            {
                Console.WriteLine(prediction);
            }

            return forecast;
        }
    }
}
