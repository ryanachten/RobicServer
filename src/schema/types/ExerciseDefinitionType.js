const graphql = require("graphql");
const { GraphQLObjectType } = graphql;
const { exerciseDefinitionFields } = require("../fields");

const ExerciseDefinitionType = new GraphQLObjectType({
  name: "ExerciseDefinitionType",
  fields: exerciseDefinitionFields
});

module.exports = ExerciseDefinitionType;
