import * as graphql from 'graphql';
import { UserDocument, UserModel, ExerciseDocument } from '../../interfaces';

import mongoose = require('mongoose');

const { GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = graphql;
const User = mongoose.model('user') as UserModel;

export default new GraphQLObjectType({
  name: 'UserType',
  fields: () => ({
    id: { type: GraphQLID },
    firstName: { type: GraphQLString },
    lastName: { type: GraphQLString },
    email: { type: GraphQLString },
    exercises: {
      type: new GraphQLList(require('./ExerciseDefinitionType')),
      resolve(parentValue: UserDocument): ExerciseDocument[] {
        return User.getExercises(parentValue.id);
      }
    }
  })
});
