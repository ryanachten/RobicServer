const graphql = require("graphql");
const { GraphQLObjectType } = graphql;
const { exerciseDefinitionFields } = require("../fields");

module.exports = new GraphQLObjectType({
  name: "ExerciseDefinitionType",
  fields: exerciseDefinitionFields
});
