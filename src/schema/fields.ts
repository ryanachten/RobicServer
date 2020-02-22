import { IExerciseDefinition } from "../interfaces";

const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLString, GraphQLID, GraphQLList, GraphQLFloat } = graphql;
const Exercise = mongoose.model("exercise");
const ExerciseDefinition = mongoose.model("exerciseDefinition");

const exerciseDefinitionFields = () => ({
  id: { type: GraphQLID },
  title: { type: GraphQLString },
  unit: { type: GraphQLString },
  primaryMuscleGroup: { type: new GraphQLList(GraphQLString) },
  type: { type: GraphQLString },
  childExercises: {
    type: new GraphQLList(require("./types/ExerciseDefinitionType")),
    resolve(parentValue: IExerciseDefinition) {
      return ExerciseDefinition.getChildExercises(parentValue.id);
    }
  },
  history: {
    type: new GraphQLList(require("./types/ExerciseType").ExerciseType),
    resolve(parentValue: IExerciseDefinition) {
      return ExerciseDefinition.getHistory(parentValue.id);
    }
  }
});

const exerciseFields = () => ({
  id: { type: GraphQLID },
  date: { type: GraphQLString },
  definition: {
    type: require("./types/ExerciseDefinitionType"),
    resolve(parentValue: IExerciseDefinition) {
      return Exercise.getDefinition(parentValue.id);
    }
  },
  netValue: { type: GraphQLFloat },
  sets: { type: new GraphQLList(require("./types/ExerciseType").SetType) },
  timeTaken: { type: GraphQLString }
});

module.exports = { exerciseFields, exerciseDefinitionFields };
