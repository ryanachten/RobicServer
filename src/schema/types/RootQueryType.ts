import * as graphql from 'graphql';
import {
  IRequest,
  UserDocument,
  ExerciseDefinitionDocument,
  ExerciseDocument
} from '../../interfaces';

const mongoose = require('mongoose');

const { GraphQLObjectType, GraphQLList, GraphQLID, GraphQLNonNull } = graphql;
const ExerciseDefinitionType = require('./ExerciseDefinitionType');
const { ExerciseType } = require('./ExerciseType');
const UserType = require('./UserType');

const User = mongoose.model('user');
const ExerciseDefinition = mongoose.model('exerciseDefinition');
const Exercise = mongoose.model('exercise');

const RootQuery = new GraphQLObjectType({
  name: 'RootQueryType',

  fields: () => ({
    currentUser: {
      description: 'Returns current user based on JSON web token from client',
      type: UserType,
      resolve(parentValue: IRequest, {}, { user }: IRequest) {
        if (!user) {
          return null;
        }
        return User.findById(user.id);
      }
    },

    getUserById: {
      type: UserType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue: UserDocument, { id }: UserDocument) {
        return User.findById(id);
      }
    },

    exerciseDefinitions: {
      type: new GraphQLList(ExerciseDefinitionType),
      resolve(parentValue: IRequest, {}, { user }: IRequest) {
        return User.getExercises(user.id);
      }
    },

    exerciseDefinition: {
      type: ExerciseDefinitionType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(
        parentValue: ExerciseDefinitionDocument,
        { id }: ExerciseDefinitionDocument
      ) {
        return ExerciseDefinition.findById(id);
      }
    },

    exercises: {
      type: new GraphQLList(ExerciseType),
      resolve() {
        return Exercise.find({});
      }
    },

    exercise: {
      type: ExerciseType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(parentValue: ExerciseDocument, { id }: ExerciseDocument) {
        return Exercise.findById(id);
      }
    }
  })
});

module.exports = RootQuery;
