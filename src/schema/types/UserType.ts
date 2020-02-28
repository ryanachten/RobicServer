import { UserDocument } from '../../interfaces';

const mongoose = require('mongoose');
const graphql = require('graphql');

const { GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = graphql;
const User = mongoose.model('user');

module.exports = new GraphQLObjectType({
  name: 'UserType',
  fields: () => ({
    id: { type: GraphQLID },
    firstName: { type: GraphQLString },
    lastName: { type: GraphQLString },
    email: { type: GraphQLString },
    exercises: {
      type: new GraphQLList(require('./ExerciseDefinitionType')),
      resolve(parentValue: UserDocument) {
        return User.getExercises(parentValue.id);
      }
    }
  })
});
