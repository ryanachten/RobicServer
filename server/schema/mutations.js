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
const SessionDefinition = mongoose.model("sessionDefinition");
const ExerciseDefinition = mongoose.model("exerciseDefinition");
const {
  SetType,
  ExerciseType,
  ExerciseDefinitionType,
  SessionDefinitionType,
  SessionType,
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

    addSessionDefinition: {
      type: SessionDefinitionType,
      args: {
        title: { type: GraphQLString },
        exercises: { type: new GraphQLList(GraphQLID) }
      },
      resolve(parentValue, { title, exercises }, { user }) {
        return new SessionDefinition({
          title,
          exercises,
          user: user.id
        }).save();
      }
    },

    addSession: {
      type: SessionType,
      args: {
        definitionId: { type: GraphQLID }
      },
      resolve(parentValue, { definitionId }) {
        return SessionDefinition.addNewSession(definitionId);
      }
    },

    addExerciseDefinition: {
      type: ExerciseDefinitionType,
      args: {
        title: { type: GraphQLString },
        unit: { type: GraphQLString }
      },
      resolve(parentValue, { title, unit }, { user }) {
        return User.createExercise(title, unit, user.id);
      }
    },

    updateExerciseDefinition: {
      type: ExerciseDefinitionType,
      args: {
        exerciseId: { type: GraphQLID },
        title: { type: GraphQLString },
        unit: { type: GraphQLString }
      },
      resolve(parentValue, { exerciseId, title, unit }) {
        return ExerciseDefinition.update(exerciseId, title, unit);
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
