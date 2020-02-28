import * as graphql from 'graphql';
import { IExercise, IExerciseDefinition, IUser, IRequest } from '../interfaces';

const {
  GraphQLList,
  GraphQLObjectType,
  GraphQLInt,
  GraphQLString,
  GraphQLID
} = graphql;
import mongoose = require('mongoose');

const Exercise = mongoose.model('exercise');
const User = mongoose.model('user');
const ExerciseDefinition = mongoose.model('exerciseDefinition');
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
        parentValue: IUser,
        { firstName, lastName, password, email }: IUser
      ) {
        return User.schema.methods.register({
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
      resolve(parentValue: IUser, { password, email }: IUser) {
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
        parentValue: IExerciseDefinition,
        {
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises
        }: IExerciseDefinition,
        { user }: IRequest
      ) {
        return User.schema.methods.createExercise({
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises,
          user: user.id
        });
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
        parentValue: { exerciseId: string } & IExerciseDefinition,
        {
          exerciseId,
          title,
          type,
          childExercises,
          unit,
          primaryMuscleGroup
        }: { exerciseId: string } & IExerciseDefinition
      ) {
        return ExerciseDefinition.schema.methods.update({
          id: exerciseId,
          title,
          unit,
          primaryMuscleGroup,
          type,
          childExercises
        });
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
      ) {
        return ExerciseDefinition.schema.methods.removeHistorySession(
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
        parentValue: { definitionId: string } & IExercise,
        { definitionId, sets, timeTaken }: { definitionId: string } & IExercise
      ) {
        return ExerciseDefinition.schema.methods.addNewSession({
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
        parentValue: { exerciseId: string } & IExercise,
        { exerciseId, sets, timeTaken }: { exerciseId: string } & IExercise
      ) {
        return Exercise.update(exerciseId, sets, timeTaken);
      }
    }
  }
});

module.exports = mutation;
