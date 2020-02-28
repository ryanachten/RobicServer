const { ExerciseType, SetType } = require('./ExerciseType');
const ExerciseDefinitionType = require('./ExerciseDefinitionType');
const UserType = require('./UserType');
const RootType = require('./RootQueryType');

module.exports = {
  RootQueryType: RootType,
  ExerciseType,
  ExerciseDefinitionType,
  SetType,
  UserType
};
