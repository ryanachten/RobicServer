const graphql = require("graphql");
const { GraphQLInputObjectType, GraphQLID, GraphQLInt, GraphQLFloat } = graphql;

const SetInput = new GraphQLInputObjectType({
  name: "SetInput",
  fields: () => ({
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat }
  })
});

module.exports = { SetInput };
