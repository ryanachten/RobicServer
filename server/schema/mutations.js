const graphql = require("graphql");
const { GraphQLObjectType, GraphQLString, GraphQLID } = graphql;
const mongoose = require("mongoose");
const User = mongoose.model("user");
const Song = mongoose.model("song");
const Lyric = mongoose.model("lyric");
const SessionDefinition = mongoose.model("sessionDefinition");
const {
  SessionDefinitionType,
  SessionType,
  UserType,
  SongType,
  LyricType
} = require("./types");

const mutation = new GraphQLObjectType({
  name: "Mutation",
  fields: {
    addUser: {
      type: UserType,
      args: {
        firstName: { type: GraphQLString },
        lastName: { type: GraphQLString },
        password: { type: GraphQLString },
        email: { type: GraphQLString }
      },
      resolve(parentValue, { firstName, lastName, password, email }) {
        return new User({
          firstName,
          lastName,
          password,
          email
        }).save();
      }
    },

    addSessionDefinition: {
      type: SessionDefinitionType,
      args: {
        title: { type: GraphQLString }
      },
      resolve(parentValue, { title }) {
        return new SessionDefinition({ title }).save();
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
