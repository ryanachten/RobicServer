const graphql = require("graphql");
const { GraphQLObjectType, GraphQLString, GraphQLID } = graphql;
const mongoose = require("mongoose");
const Song = mongoose.model("song");
const Lyric = mongoose.model("lyric");
const SessionDefinition = mongoose.model("sessionDefinition");
const {
  SongType,
  LyricType,
  SessionDefinitionType,
  SessionType
} = require("./types");

const mutation = new GraphQLObjectType({
  name: "Mutation",
  fields: {
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
