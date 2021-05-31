﻿using MediatR;
using Microsoft.ML;
using RobicServer.Models;
using RobicServer.Query;
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
            IDataView trainData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", upperBound: 1);
            IDataView evaluationData = _mlContext.Data.FilterRowsByColumn(dataView, "IsTrainingInput", lowerBound: 1);

            //var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
            //    outputColumnName: "ForecastedNetValue",
            //    inputColumnName: "NetValue",
            //    windowSize: 7,
            //    seriesLength: 30,
            //    trainSize: 365,
            //    horizon: 7,
            //    confidenceLevel: 0.95f,
            //    confidenceLowerBoundColumn: "LowerBoundNetValue",
            //    confidenceUpperBoundColumn: "UpperBoundNetValue");
        }
    }
}
