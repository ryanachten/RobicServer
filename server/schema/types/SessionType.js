const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = graphql;
const { ExerciseType } = require("./ExerciseType");
const Session = mongoose.model("session");

const SessionType = new GraphQLObjectType({
  name: "SessionType",
  fields: () => ({
    id: { type: GraphQLID },
    date: { type: GraphQLString },
    definition: {
      type: require("./SessionDefinitionType"),
      resolve(parentValue) {
        return Session.getDefinition(parentValue.id);
      }
    },
    exercises: {
      type: new GraphQLList(ExerciseType),
      resolve(parentValue) {
        return Session.getExercises(parentValue.id);
      }
    }
  })
});

module.exports = SessionType;
