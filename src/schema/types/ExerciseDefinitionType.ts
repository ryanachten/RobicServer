import * as graphql from "graphql";
const { exerciseDefinitionFields } = require("../fields");

const { GraphQLObjectType } = graphql;

module.exports = new GraphQLObjectType({
  name: "ExerciseDefinitionType",
  fields: exerciseDefinitionFields
});
