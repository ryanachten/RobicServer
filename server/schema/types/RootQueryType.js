const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLList, GraphQLID, GraphQLNonNull } = graphql;
const SessionDefinitionType = require("./SessionDefinitionType");
const SessionType = require("./SessionType");
const SongType = require("./SongType");
const LyricType = require("./LyricType");

const SessionDefinition = mongoose.model("sessionDefinition");
const Session = mongoose.model("session");
const Lyric = mongoose.model("lyric");
const Song = mongoose.model("song");

const RootQuery = new GraphQLObjectType({
  name: "RootQueryType",
  fields: () => ({
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
