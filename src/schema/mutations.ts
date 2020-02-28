import * as graphql from 'graphql';
import {
  ExerciseDocument,
  ExerciseModel,
  ExerciseDefinitionDocument,
  ExerciseDefinitionModel,
  Request,
  UserDocument,
  UserModel
} from '../interfaces';

const {
  GraphQLList,
  GraphQLObjectType,
  GraphQLInt,
  GraphQLString,
  GraphQLID
} = graphql;
import mongoose = require('mongoose');

const Exercise = mongoose.model('exercise') as ExerciseModel;
const User = mongoose.model('user') as UserModel;
const ExerciseDefinition = mongoose.model(
  'exerciseDefinition'
) as ExerciseDefinitionModel;
const { ExerciseType, ExerciseDefinitionType, UserType } = require('./types');
const { SetInput } = require('./inputs');

const mutation = new GraphQLObjectType({
  name: 'Mutation',
  fields: {
    registerUser: {
      type: UserType,
      args: {
        firstName: { type: GraphQLString },
        lastName: { type: GraphQLString },
        password: { type: GraphQLString },
        email: { type: GraphQLString }
      },
      resolve(
        parentValue: UserDocument,
        { firstName, lastName, password, email }: UserDocument
      ): UserDocument {
        return User.register({
          firstName,
          lastName,
          password,
          email
        });
      }
    },

    loginUser: {
      type: GraphQLString,
      args: {
        password: { type: GraphQLString },
        email: { type: GraphQLString }
      },
      resolve(
        parentValue: UserDocument,
        { password, email }: UserDocument
      ): UserDocument {
        return User.login({ password, email });
      }
    },

    addExerciseDefinition: {
      type: ExerciseDefinitionType,
      args: {
        title: { type: GraphQLString },
        unit: { type: GraphQLString },
        type: { type: GraphQLString },
        childExercises: { type: new GraphQLList(GraphQLID) },
        primaryMuscleGroup: { type: new GraphQLList(GraphQLString) }
      },
      resolve(
        parentValue: ExerciseDefinitionDocument,
        {
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises
        }: ExerciseDefinitionDocument,
        { user }: Request
      ): ExerciseDefinitionDocument {
        const exercise = {
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises,
          user: user.id
        } as ExerciseDefinitionDocument;
        return User.createExercise(exercise);
      }
    },

    updateExerciseDefinition: {
      type: ExerciseDefinitionType,
      args: {
        exerciseId: { type: GraphQLID },
        title: { type: GraphQLString },
        unit: { type: GraphQLString },
        type: { type: GraphQLString },
        childExercises: { type: new GraphQLList(GraphQLID) },
        primaryMuscleGroup: { type: new GraphQLList(GraphQLString) }
      },
      resolve(
        parentValue: { exerciseId: string } & ExerciseDefinitionDocument,
        {
          exerciseId,
          title,
          type,
          childExercises,
          unit,
          primaryMuscleGroup
        }: { exerciseId: string } & ExerciseDefinitionDocument
      ): ExerciseDefinitionDocument {
        const exercise = {
          id: exerciseId,
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises
        } as ExerciseDefinitionDocument;
        return ExerciseDefinition.update(exercise);
      }
    },

    removeHistorySession: {
      type: ExerciseDefinitionType,
      args: {
        definitionId: { type: GraphQLID },
        exerciseId: { type: GraphQLID }
      },
      resolve(
        parentValue: { definitionId: string; exerciseId: string },
        {
          definitionId,
          exerciseId
        }: { definitionId: string; exerciseId: string }
      ): ExerciseDefinitionDocument {
        return ExerciseDefinition.removeHistorySession(
          definitionId,
          exerciseId
        );
      }
    },

    addExercise: {
      type: ExerciseType,
      args: {
        definitionId: { type: GraphQLID },
        sets: { type: new GraphQLList(SetInput) },
        timeTaken: { type: GraphQLString }
      },
      resolve(
        parentValue: { definitionId: string } & ExerciseDocument,
        {
          definitionId,
          sets,
          timeTaken
        }: { definitionId: string } & ExerciseDocument
      ): ExerciseDocument {
        return ExerciseDefinition.addNewSession({
          definitionId,
          sets,
          timeTaken
        });
      }
    },

    updateExercise: {
      type: ExerciseType,
      args: {
        exerciseId: { type: GraphQLID },
        sets: { type: new GraphQLList(SetInput) },
        timeTaken: { type: GraphQLInt }
      },
      resolve(
        parentValue: { exerciseId: string } & ExerciseDocument,
        {
          exerciseId,
          sets,
          timeTaken
        }: { exerciseId: string } & ExerciseDocument
      ): ExerciseModel {
        return Exercise.update(exerciseId, sets, timeTaken);
      }
    }
  }
});

module.exports = mutation;
