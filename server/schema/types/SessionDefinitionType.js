const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = graphql;
// const SessionType = require("./SessionType");
const SessionDefinition = mongoose.model("sessionDefinition");

const SessionDefinitionType = new GraphQLObjectType({
  name: "SessionDefinitionType",
  fields: () => ({
    id: { type: GraphQLID },
    title: { type: GraphQLString }
    // history: {
    //   type: new GraphQLList(SessionType),
    //   resolve(parentValue) {
    //     return SessionDefinition.getHistory(parentValue.id);
    //   }
    // }
  })
});

module.exports = SessionDefinitionType;
