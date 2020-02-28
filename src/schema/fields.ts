import * as graphql from 'graphql';
import {
  ExerciseDefinitionDocument,
  ExerciseDefinitionModel,
  ExerciseDocument,
  ExerciseModel
} from '../interfaces';

import mongoose = require('mongoose');

const { GraphQLString, GraphQLID, GraphQLList, GraphQLFloat } = graphql;
const Exercise = mongoose.model('exercise') as ExerciseModel;
const ExerciseDefinition = mongoose.model(
  'exerciseDefinition'
) as ExerciseDefinitionModel;

const exerciseDefinitionFields = () => ({
  id: { type: GraphQLID },
  title: { type: GraphQLString },
  unit: { type: GraphQLString },
  primaryMuscleGroup: { type: new GraphQLList(GraphQLString) },
  type: { type: GraphQLString },
  childExercises: {
    type: new GraphQLList(require('./types/ExerciseDefinitionType')),
    resolve(
      parentValue: ExerciseDefinitionDocument
    ): ExerciseDefinitionDocument[] {
      return ExerciseDefinition.getChildExercises(parentValue.id);
    }
  },
  history: {
    type: new GraphQLList(require('./types/ExerciseType').ExerciseType),
    resolve(parentValue: ExerciseDefinitionDocument): ExerciseDocument[] {
      return ExerciseDefinition.getHistory(parentValue.id);
    }
  }
});

const exerciseFields = () => ({
  id: { type: GraphQLID },
  date: { type: GraphQLString },
  definition: {
    type: require('./types/ExerciseDefinitionType'),
    resolve(
      parentValue: ExerciseDefinitionDocument
    ): ExerciseDefinitionDocument {
      return Exercise.getDefinition(parentValue.id);
    }
  },
  netValue: { type: GraphQLFloat },
  sets: { type: new GraphQLList(require('./types/ExerciseType').SetType) },
  timeTaken: { type: GraphQLString }
});

module.exports = { exerciseFields, exerciseDefinitionFields };
