import * as graphql from 'graphql';
import { ExerciseDefinitionDocument } from '../../interfaces';

import mongoose = require('mongoose');
const {
  GraphQLObjectType,
  GraphQLString,
  GraphQLID,
  GraphQLList,
  GraphQLInt,
  GraphQLFloat
} = graphql;
const ExerciseDefinition = mongoose.model('exerciseDefinition');
const { exerciseFields } = require('../fields');

const SetExercise = new GraphQLObjectType({
  name: 'SetExercise',
  fields: () => ({
    id: { type: GraphQLID },
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat },
    unit: {
      type: GraphQLString,
      resolve(
        parentValue: ExerciseDefinitionDocument,
        { id }: ExerciseDefinitionDocument
      ) {
        return ExerciseDefinition.schema.methods.getUnit(parentValue.id);
      }
    }
  })
});

const SetType = new GraphQLObjectType({
  name: 'SetType',
  fields: () => ({
    exercises: {
      type: new GraphQLList(SetExercise)
    },
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat }
  })
});

const ExerciseType = new GraphQLObjectType({
  name: 'ExerciseType',
  fields: exerciseFields
});

module.exports = { ExerciseType, SetType };
