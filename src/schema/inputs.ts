const graphql = require('graphql');

const {
  GraphQLInputObjectType,
  GraphQLID,
  GraphQLInt,
  GraphQLList,
  GraphQLFloat,
} = graphql;
const { exerciseFields, exerciseDefinitionFields } = require('./fields');

const SetExerciseInput = new GraphQLInputObjectType({
  name: 'SetExerciseInput',
  fields: () => ({
    id: { type: GraphQLID },
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat },
  }),
});

const SetInput = new GraphQLInputObjectType({
  name: 'SetInput',
  fields: () => ({
    exercises: {
      type: new GraphQLList(SetExerciseInput),
    },
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat },
  }),
});

const ExerciseDefinitionInput = new GraphQLInputObjectType({
  name: 'ExerciseDefinitionInput',
  fields: () => ({
    ...exerciseDefinitionFields(),
    childExercises: {
      type: ExerciseDefinitionInput,
    },
    history: {
      type: require('./inputs').ExerciseInput,
    },
  }),
});

const ExerciseInput = new GraphQLInputObjectType({
  name: 'ExerciseInput',
  fields: () => ({
    ...exerciseFields(),
    definition: {
      type: ExerciseDefinitionInput,
    },
    sets: {
      type: SetInput,
    },
  }),
});

module.exports = { ExerciseDefinitionInput, ExerciseInput, SetInput };
