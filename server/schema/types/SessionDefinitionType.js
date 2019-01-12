const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = graphql;
const ExerciseDefinitionType = require("./ExerciseDefinitionType");
const SessionType = require("./SessionType");
const SessionDefinition = mongoose.model("sessionDefinition");

const SessionDefinitionType = new GraphQLObjectType({
  name: "SessionDefinitionType",
  fields: () => ({
    id: { type: GraphQLID },
    title: { type: GraphQLString },
    exercises: {
      type: new GraphQLList(ExerciseDefinitionType),
      resolve(parentValue) {
        return SessionDefinition.getExercises(parentValue.id);
      }
    },
    history: {
      type: new GraphQLList(SessionType),
      resolve(parentValue) {
        return SessionDefinition.getHistory(parentValue.id);
      }
    }
  })
});

module.exports = SessionDefinitionType;
