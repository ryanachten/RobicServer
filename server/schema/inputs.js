const graphql = require("graphql");
const {
  GraphQLInputObjectType,
  GraphQLID,
  GraphQLInt,
  GraphQLList,
  GraphQLFloat
} = graphql;

const SetExerciseInput = new GraphQLInputObjectType({
  name: "SetExerciseInput",
  fields: () => ({
    id: { type: GraphQLID },
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat }
  })
});

const SetInput = new GraphQLInputObjectType({
  name: "SetInput",
  fields: () => ({
    exercises: {
      type: new GraphQLList(SetExerciseInput)
    },
    reps: { type: GraphQLInt },
    value: { type: GraphQLFloat }
  })
});

module.exports = { SetInput };
