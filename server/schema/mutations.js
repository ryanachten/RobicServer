const graphql = require("graphql");
const { GraphQLList, GraphQLObjectType, GraphQLString, GraphQLID } = graphql;
const mongoose = require("mongoose");
const User = mongoose.model("user");
const Song = mongoose.model("song");
const Lyric = mongoose.model("lyric");
const SessionDefinition = mongoose.model("sessionDefinition");
const ExerciseDefinition = mongoose.model("exerciseDefinition");
const {
  ExerciseType,
  ExerciseDefinitionType,
  SessionDefinitionType,
  SessionType,
  UserType,
  SongType,
  LyricType
} = require("./types");

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
        return new ExerciseDefinition({ title, unit, user: user.id }).save();
      }
    },

    addExercise: {
      type: ExerciseType,
      args: {
        definitionId: { type: GraphQLID },
        sessionId: { type: GraphQLID }
      },
      resolve(parentValue, { definitionId, sessionId }) {
        return ExerciseDefinition.addNewSession(definitionId, sessionId);
      }
    },

    // TODO: delete following
    addSong: {
      type: SongType,
      args: {
        title: { type: GraphQLString }
      },
      resolve(parentValue, { title }) {
        return new Song({ title }).save();
      }
    },
    addLyricToSong: {
      type: SongType,
      args: {
        content: { type: GraphQLString },
        songId: { type: GraphQLID }
      },
      resolve(parentValue, { content, songId }) {
        return Song.addLyric(songId, content);
      }
    },
    likeLyric: {
      type: LyricType,
      args: { id: { type: GraphQLID } },
      resolve(parentValue, { id }) {
        return Lyric.like(id);
      }
    },
    deleteSong: {
      type: SongType,
      args: { id: { type: GraphQLID } },
      resolve(parentValue, { id }) {
        return Song.remove({ _id: id });
      }
    }
  }
});

module.exports = mutation;
