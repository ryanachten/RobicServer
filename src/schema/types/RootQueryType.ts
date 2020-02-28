import * as graphql from 'graphql';
import {
  Request,
  UserDocument,
  ExerciseDefinitionDocument,
  ExerciseDocument,
  UserModel
} from '../../interfaces';
import UserType from './UserType';

import mongoose = require('mongoose');

const { GraphQLObjectType, GraphQLList, GraphQLID, GraphQLNonNull } = graphql;
const ExerciseDefinitionType = require('./ExerciseDefinitionType');
const { ExerciseType } = require('./ExerciseType');

const User = mongoose.model('user') as UserModel;
const ExerciseDefinition = mongoose.model('exerciseDefinition');
const Exercise = mongoose.model('exercise');

const RootQuery = new GraphQLObjectType({
  name: 'RootQueryType',

  fields: () => ({
    currentUser: {
      description: 'Returns current user based on JSON web token from client',
      type: UserType,
      resolve(
        parentValue: Request,
        {},
        { user }: Request
      ): mongoose.DocumentQuery<UserDocument, UserDocument> {
        if (!user) {
          return null;
        }
        return User.findById(user.id);
      }
    },

    getUserById: {
      type: UserType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(
        parentValue: UserDocument,
        { id }: UserDocument
      ): mongoose.DocumentQuery<UserDocument, UserDocument> {
        return User.findById(id);
      }
    },

    exerciseDefinitions: {
      type: new GraphQLList(ExerciseDefinitionType),
      resolve(parentValue: Request, {}, { user }: Request): ExerciseDocument[] {
        return User.getExercises(user.id);
      }
    },

    exerciseDefinition: {
      type: ExerciseDefinitionType,
      args: { id: { type: new GraphQLNonNull(GraphQLID) } },
      resolve(
        parentValue: ExerciseDefinitionDocument,
        { id }: ExerciseDefinitionDocument
      ): mongoose.DocumentQuery<mongoose.Document, mongoose.Document> {
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
      resolve(
        parentValue: ExerciseDocument,
        { id }: ExerciseDocument
      ): mongoose.DocumentQuery<mongoose.Document, mongoose.Document> {
        return Exercise.findById(id);
      }
    }
  })
});

module.exports = RootQuery;
