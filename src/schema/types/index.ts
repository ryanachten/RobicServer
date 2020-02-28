import UserType from './UserType';

const { ExerciseType, SetType } = require('./ExerciseType');
const ExerciseDefinitionType = require('./ExerciseDefinitionType');

const RootType = require('./RootQueryType');

module.exports = {
  RootQueryType: RootType,
  ExerciseType,
  ExerciseDefinitionType,
  SetType,
  UserType
};
