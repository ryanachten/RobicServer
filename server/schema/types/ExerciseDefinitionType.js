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
const UserType = require("./UserType");
const { ExerciseType } = require("./ExerciseType");
const ExerciseDefinition = mongoose.model("exerciseDefinition");

const BestValueType = new GraphQLObjectType({
  name: "BestValueType",
  fields: () => ({
    value: { type: GraphQLFloat },
    exercise: {
      type: ExerciseType,
      resolve(parentValue) {
        return ExerciseDefinition.getPersonalBestExercise(
          parentValue.id,
          "value"
        );
      }
    }
  })
});

const BestSetCountType = new GraphQLObjectType({
  name: "BestSetCountType",
  fields: () => ({
    value: { type: GraphQLInt },
    exercise: {
      type: ExerciseType,
      resolve(parentValue) {
        return ExerciseDefinition.getPersonalBestExercise(
          parentValue.id,
          "setCount"
        );
      }
    }
  })
});

const BestTotalRepsType = new GraphQLObjectType({
  name: "BestTotalRepsType",
  fields: () => ({
    value: { type: GraphQLInt },
    exercise: {
      type: ExerciseType,
      resolve(parentValue) {
        return ExerciseDefinition.getPersonalBestExercise(
          parentValue.id,
          "totalReps"
        );
      }
    }
  })
});

const BestNetValueType = new GraphQLObjectType({
  name: "BestNetValueType",
  fields: () => ({
    value: { type: GraphQLFloat },
    exercise: {
      type: ExerciseType,
      resolve(parentValue) {
        return ExerciseDefinition.getPersonalBestExercise(
          parentValue.id,
          "netValue"
        );
      }
    }
  })
});

const BestTimeTakenType = new GraphQLObjectType({
  name: "BestTimeTakenType",
  fields: () => ({
    value: { type: GraphQLFloat },
    exercise: {
      type: ExerciseType,
      resolve(parentValue) {
        return ExerciseDefinition.getPersonalBestExercise(
          parentValue.id,
          "timeTaken"
        );
      }
    }
  })
});

const PersonalBestType = new GraphQLObjectType({
  name: "PersonalBestType",
  fields: () => ({
    value: { type: BestValueType },
    setCount: { type: BestSetCountType },
    totalReps: { type: BestTotalRepsType },
    netValue: { type: BestNetValueType },
    timeTaken: { type: BestTimeTakenType }
  })
});

const ExerciseDefinitionType = new GraphQLObjectType({
  name: "ExerciseDefinitionType",
  fields: () => ({
    id: { type: GraphQLID },
    title: { type: GraphQLString },
    unit: { type: GraphQLString },
    primaryMuscleGroup: { type: new GraphQLList(GraphQLString) },
    personalBest: { type: PersonalBestType },
    type: { type: GraphQLString },
    childExercises: {
      type: new GraphQLList(ExerciseDefinitionType),
      resolve(parentValue) {
        return ExerciseDefinition.getChildExercises(parentValue.id);
      }
    },
    history: {
      type: new GraphQLList(ExerciseType),
      resolve(parentValue) {
        return ExerciseDefinition.getHistory(parentValue.id);
      }
    }
  })
});

module.exports = ExerciseDefinitionType;
