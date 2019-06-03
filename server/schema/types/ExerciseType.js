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
    sets: { type: new GraphQLList(SetType) },
    netValue: { type: GraphQLFloat }
  })
});

module.exports = { ExerciseType, SetType };
