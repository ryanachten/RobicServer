const _ = require("lodash");
require("graphql");
const { GraphQLSchema } = graphql;

const { RootQueryType } = require("./types");
const mutations = require("./mutations");

module.exports = new GraphQLSchema({
  query: RootQueryType,
  mutation: mutations
});
