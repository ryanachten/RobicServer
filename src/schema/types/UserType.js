const mongoose = require("mongoose");
const graphql = require("graphql");
const { GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = graphql;
const User = mongoose.model("user");

const UserType = new GraphQLObjectType({
  name: "UserType",
  fields: () => ({
    id: { type: GraphQLID },
    firstName: { type: GraphQLString },
    lastName: { type: GraphQLString },
    email: { type: GraphQLString },
    exercises: {
      type: new GraphQLList(require("./ExerciseDefinitionType")),
      resolve(parentValue) {
        return User.getExercises(parentValue.id);
      }
    }
  })
});

module.exports = UserType;