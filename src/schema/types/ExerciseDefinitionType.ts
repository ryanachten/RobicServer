import * as graphql from 'graphql';

const { exerciseDefinitionFields } = require('../fields');

const { GraphQLObjectType } = graphql;

const ExerciseDefinition = new GraphQLObjectType({
  name: 'ExerciseDefinitionType',
  fields: exerciseDefinitionFields
});

module.exports = ExerciseDefinition;
