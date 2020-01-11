const graphql = require("graphql");
const {
  GraphQLList,
  GraphQLObjectType,
  GraphQLInt,
  GraphQLString,
  GraphQLID
} = graphql;
const mongoose = require("mongoose");
const Exercise = mongoose.model("exercise");
const User = mongoose.model("user");
const ExerciseDefinition = mongoose.model("exerciseDefinition");
const {
  SetType,
  ExerciseType,
  ExerciseDefinitionType,
  UserType
} = require("./types");
const { SetInput } = require("./inputs");

const mutation = new GraphQLObjectType({
  name: "Mutation",
  fields: {
    registerUser: {
      type: UserType,
      args: {
        firstName: { type: GraphQLString },
        lastName: { type: GraphQLString },
        password: { type: GraphQLString },
        email: { type: GraphQLString }
      },
      resolve(parentValue, { firstName, lastName, password, email }) {
        return User.register({ firstName, lastName, password, email });
      }
    },

    loginUser: {
      type: GraphQLString,
      args: {
        password: { type: GraphQLString },
        email: { type: GraphQLString }
      },
      resolve(parentValue, { password, email }) {
        return User.login({ password, email });
      }
    },

    addExerciseDefinition: {
      type: ExerciseDefinitionType,
      args: {
        title: { type: GraphQLString },
        unit: { type: GraphQLString },
        type: { type: GraphQLString },
        childExercises: { type: new GraphQLList(GraphQLID) },
        primaryMuscleGroup: { type: new GraphQLList(GraphQLString) }
      },
      resolve(
        parentValue,
        { title, unit, primaryMuscleGroup, type, childExercises },
        { user }
      ) {
        return User.createExercise({
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises,
          user: user.id
        });
      }
    },

    updateExerciseDefinition: {
      type: ExerciseDefinitionType,
      args: {
        exerciseId: { type: GraphQLID },
        title: { type: GraphQLString },
        unit: { type: GraphQLString },
        type: { type: GraphQLString },
        childExercises: { type: new GraphQLList(GraphQLID) },
        primaryMuscleGroup: { type: new GraphQLList(GraphQLString) }
      },
      resolve(
        parentValue,
        { exerciseId, title, type, childExercises, unit, primaryMuscleGroup }
      ) {
        return ExerciseDefinition.update({
          id: exerciseId,
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises
        });
      }
    },

    removeHistorySession: {
      type: ExerciseDefinitionType,
      args: {
        definitionId: { type: GraphQLID },
        exerciseId: { type: GraphQLID }
      },
      resolve(parentValue, { definitionId, exerciseId }) {
        return ExerciseDefinition.removeHistorySession(
          definitionId,
          exerciseId
        );
      }
    },

    addExercise: {
      type: ExerciseType,
      args: {
        definitionId: { type: GraphQLID },
        sets: { type: new GraphQLList(SetInput) },
        timeTaken: { type: GraphQLString }
      },
      resolve(parentValue, { definitionId, sets, timeTaken }) {
        return ExerciseDefinition.addNewSession({
          definitionId,
          sets,
          timeTaken
        });
      }
    },

    updateExercise: {
      type: ExerciseType,
      args: {
        exerciseId: { type: GraphQLID },
        sets: { type: new GraphQLList(SetInput) },
        timeTaken: { type: GraphQLInt }
      },
      resolve(parentValue, { exerciseId, sets, timeTaken }) {
        return Exercise.update(exerciseId, sets, timeTaken);
      }
    }
  }
});

module.exports = mutation;
