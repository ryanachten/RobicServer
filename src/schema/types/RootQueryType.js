const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLList, GraphQLID, GraphQLNonNull } = graphql;
const { ExerciseType } = require("./ExerciseType");
const ExerciseDefinitionType = require("./ExerciseDefinitionType");
const UserType = require("./UserType");

const User = mongoose.model("user");
const ExerciseDefinition = mongoose.model("exerciseDefinition");
const Exercise = mongoose.model("exercise");

const RootQuery = new GraphQLObjectType({
  name: "RootQueryType",

  fields: () => ({
    currentUser: {
      description: "Returns current user based on JSON web token from client",
      type: UserType,
      resolve(parentValue, {}, { user }) {
        if (!user) {
          return null;
        }
        return User.findById(user.id);
      }
    },

    getUserById: {
      type: UserType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue, { id }) {
        return User.findById(id);
      }
    },

    exerciseDefinitions: {
      type: new GraphQLList(ExerciseDefinitionType),
      resolve(parentValue, {}, { user }) {
        return User.getExercises(user.id);
      }
    },

    exerciseDefinition: {
      type: ExerciseDefinitionType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue, { id }) {
        return ExerciseDefinition.findById(id);
      }
    },

    exercises: {
      type: new GraphQLList(ExerciseType),
      resolve() {
        return Exercise.find({});
      }
    },

    exercise: {
      type: ExerciseType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue, { id }) {
        return Exercise.findById(id);
      }
    }
  })
});

module.exports = RootQuery;
