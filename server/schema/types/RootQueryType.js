const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLList, GraphQLID, GraphQLNonNull } = graphql;
const { ExerciseType } = require("./ExerciseType");
const ExerciseDefinitionType = require("./ExerciseDefinitionType");
const SessionDefinitionType = require("./SessionDefinitionType");
const SessionType = require("./SessionType");
const UserType = require("./UserType");

const SongType = require("./SongType");
const LyricType = require("./LyricType");

const User = mongoose.model("user");
const SessionDefinition = mongoose.model("sessionDefinition");
const Session = mongoose.model("session");
const ExerciseDefinition = mongoose.model("exerciseDefinition");
const Exercise = mongoose.model("exercise");

const Lyric = mongoose.model("lyric");
const Song = mongoose.model("song");

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
    },

    sessionDefinitions: {
      type: new GraphQLList(SessionDefinitionType),
      resolve() {
        return SessionDefinition.find({});
      }
    },

    sessionDefinition: {
      type: SessionDefinitionType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue, { id }) {
        return SessionDefinition.findById(id);
      }
    },

    sessions: {
      type: new GraphQLList(SessionType),
      resolve() {
        return Session.find({});
      }
    },

    session: {
      type: SessionType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue, { id }) {
        return Session.findById(id);
      }
    },

    // TODO: delete following
    songs: {
      type: new GraphQLList(SongType),
      resolve() {
        return Song.find({});
      }
    },
    song: {
      type: SongType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue, { id }) {
        return Song.findById(id);
      }
    },
    lyric: {
      type: LyricType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parnetValue, { id }) {
        return Lyric.findById(id);
      }
    }
  })
});

module.exports = RootQuery;
