const mongoose = require("mongoose");
const graphql = require("graphql");
const {
  GraphQLObjectType,
  GraphQLString,
  GraphQLID,
  GraphQLList,
  GraphQLInt,
  GraphQLFloat
} = graphql;
const ExerciseDefinition = mongoose.model("exerciseDefinition");

const exerciseDefinitionFields = () => ({
  id: { type: GraphQLID },
  title: { type: GraphQLString },
  unit: { type: GraphQLString },
  primaryMuscleGroup: { type: new GraphQLList(GraphQLString) },
  type: { type: GraphQLString },
  childExercises: {
    type: new GraphQLList(require("./types/ExerciseDefinitionType")),
    resolve(parentValue) {
      return ExerciseDefinition.getChildExercises(parentValue.id);
    }
  },
  history: {
    type: new GraphQLList(require("./types/ExerciseType").ExerciseType),
    resolve(parentValue) {
      return ExerciseDefinition.getHistory(parentValue.id);
    }
  }
});

exerciseFields = () => ({
  id: { type: GraphQLID },
  date: { type: GraphQLString },
  definition: {
    type: require("./types/ExerciseDefinitionType"),
    resolve(parentValue) {
      return Exercise.getDefinition(parentValue.id);
    }
  },
  netValue: { type: GraphQLFloat },
  sets: { type: new GraphQLList(require("./types/ExerciseType").SetType) },
  timeTaken: { type: GraphQLString }
});

module.exports = { exerciseFields, exerciseDefinitionFields };
