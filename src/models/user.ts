import validator from 'validator';
import {
  UserDocument,
  UserModel,
  ExerciseDefinitionDocument,
  ExerciseDocument
} from '../interfaces';

import _ = require('lodash');
import bcrypt = require('bcrypt');
import jwt = require('jsonwebtoken');
import mongoose = require('mongoose');

const { Schema } = mongoose;

// Extract secret from env for password salt
require('dotenv').config();

const { JWT_PASSWORD_SECRET } = process.env;

/*
  Houses user information and authentication details
*/

const UserSchema = new Schema({
  // emails must be unique and valid emails
  email: {
    type: String,
    required: true,
    trim: true,
    minlength: 1,
    unique: true,
    validate: {
      validator: (value: string): boolean => validator.isEmail(value),
      message: '{VALUE} is not a valid email'
    }
  },
  password: { type: String, required: true, minlength: 6 },
  tokens: [
    {
      access: {
        type: String,
        required: true
      },
      token: {
        type: String,
        required: true
      }
    }
  ],
  firstName: { type: String },
  lastName: { type: String },
  exercises: [
    {
      type: Schema.Types.ObjectId,
      ref: 'exerciseDefinition'
    }
  ]
});

UserSchema.statics.register = async ({
  firstName,
  lastName,
  password,
  email
}: UserDocument): Promise<UserDocument> => {
  const User = mongoose.model('user');
  // Store user password using hashed password with 12 salt rounds
  const hashedPassword = await bcrypt.hash(password, 12);
  const newUser = (await new User({
    firstName,
    lastName,
    email,
    password: hashedPassword,
    exercises: []
  }).save()) as UserDocument;
  return newUser;
};

UserSchema.statics.login = async ({
  password,
  email
}: UserDocument): Promise<string> => {
  const User = mongoose.model('user');
  // Locate user by email address in DB.
  const locatedUser = (await User.findOne({
    email
  })) as UserDocument;
  if (!locatedUser) {
    throw new Error(`No user with the email address ${email} was found`);
  }
  // Compared the provided email with the hashed version stored on the user object
  const validPassword = await bcrypt.compare(password, locatedUser.password);
  if (!validPassword) {
    throw new Error('The password provided is incorrect');
  }
  // Create signed JSON web token
  const token = jwt.sign(
    {
      // At this stage, only include user ID in token to minimise decodable information in token
      user: _.pick(locatedUser, ['id'])
    },
    JWT_PASSWORD_SECRET,
    {
      // Token will expire in one year
      expiresIn: '1y'
    }
  );
  return token;
};

UserSchema.statics.getExercises = function(id: string): ExerciseDocument[] {
  return this.findById(id)
    .populate('exercises')
    .then((user: UserDocument) => user.exercises);
};

UserSchema.statics.createExercise = async function({
  title,
  unit,
  primaryMuscleGroup,
  type,
  childExercises,
  user: userId
}: ExerciseDefinitionDocument): Promise<ExerciseDocument> {
  const ExerciseDefinition = mongoose.model('exerciseDefinition');
  const user = await this.findById(userId);
  // Create and save the new definition
  const definition = (await new ExerciseDefinition({
    title,
    unit,
    primaryMuscleGroup,
    type,
    childExercises,
    user
  }).save()) as ExerciseDocument;
  // Add the exercise to the user's exercises
  await user.populate('exercises');
  user.exercises.push(definition);
  await user.save();
  return definition;
};

mongoose.model<UserDocument, UserModel>('user', UserSchema);
