const mongoose = require("mongoose");
const graphql = require("graphql");
const {
  GraphQLObjectType,
  GraphQLString,
  GraphQLID,
  GraphQLList,
  GraphQLInt,
  GraphQLFloat
} = graphql;
const Exercise = mongoose.model("exercise");

const SetType = new GraphQLObjectType({
  name: "SetType",
  fields: () => ({
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat }
  })
});

const ExerciseType = new GraphQLObjectType({
  name: "ExerciseType",
  fields: () => ({
    id: { type: GraphQLID },
    date: { type: GraphQLString },
    definition: {
      type: require("./ExerciseDefinitionType"),
      resolve(parentValue) {
        return Exercise.getDefinition(parentValue.id);
      }
    },
    netValue: { type: GraphQLFloat },
    sets: { type: new GraphQLList(SetType) },
    timeTaken: { type: GraphQLString }
  })
});

module.exports = { ExerciseType, SetType };
