const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = graphql;
const UserType = require("./UserType");
const ExerciseType = require("./ExerciseType");
const ExerciseDefinition = mongoose.model("exerciseDefinition");

// TODO: these string types should be objects associated to ExerciseType``
const PersonalBestType = new GraphQLObjectType({
  name: "PersonalBestType",
  fields: () => ({
    value: { type: GraphQLString },
    setCount: { type: GraphQLString },
    totalReps: { type: GraphQLString },
    timeTaken: { type: GraphQLString },
    netValue: { type: GraphQLString }
  })
});

const ExerciseDefinitionType = new GraphQLObjectType({
  name: "ExerciseDefinitionType",
  fields: () => ({
    id: { type: GraphQLID },
    title: { type: GraphQLString },
    unit: { type: GraphQLString },
    personalBest: { type: PersonalBestType },
    history: {
      type: new GraphQLList(ExerciseType),
      resolve(parentValue) {
        return ExerciseDefinition.getHistory(parentValue.id);
      }
    }
  })
});

module.exports = ExerciseDefinitionType;
