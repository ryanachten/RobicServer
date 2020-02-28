import { Document, Model } from 'mongoose';

export type IRequest = {
  headers: {
    authorization: string;
  };
  user: UserDocument;
  next: () => void;
};

export enum MuscleGroup {
  CHEST = 'Chest',
  FOREARMS = 'Forearms',
  LATS = 'Lats',
  // MIDDLE_BACK = "Middle Back",
  LOWER_BACK = 'Lower Back',
  NECK = 'Neck',
  HAMS = 'Hamstrings',
  QUADS = 'Quadriceps',
  CALVES = 'Calves',
  TRICEPS = 'Triceps',
  TRAPS = 'Traps',
  SHOULDERS = 'Shoulders',
  ABS = 'Abdominals',
  OBLIQUES = 'Obliques',
  GLUTES = 'Glutes',
  BICEPS = 'Biceps'
  // ADDUCTORS = "Adductors",
  // ABDUCTORS = "Abductors"
}

export enum Unit {
  kg = 'kg',
  min = 'min',
  bodyweight = 'body weight'
}

export interface SetExercise {
  id: string;
  reps: number;
  value: number;
  unit: Unit;
}

export type ISet = {
  exercises?: SetExercise[];
  reps: number; // TODO: should be optional
  value: number; // TODO: should be optional
};

export enum ExerciseType {
  CIRCUIT = 'Circuit',
  STANDARD = 'Standard',
  SUPERSET = 'Superset'
}

export type PersonalBest = {
  netValue: { value: number };
  setCount: { value: number };
  timeTaken: { value: number };
  totalReps: { value: number };
  value: { value: number };
};

export interface ExerciseDocument extends Document {
  id: string;
  date: string;
  definition: ExerciseDefinitionDocument;
  sets: ISet[];
  timeTaken: string;
}

export interface ExerciseModel extends Model<ExerciseDocument> {
  // FIXME: occluded by Model.update
  //  should be - update: (exerciseId: string, sets: ISet[], timeTaken: string) => ExerciseDocument
  update: (exerciseId: any, sets: any, timeTaken: any) => any;
}

export interface ExerciseDefinitionDocument extends Document {
  childExercises?: ExerciseDefinitionDocument[];
  history: ExerciseDocument[];
  id: string;
  primaryMuscleGroup: MuscleGroup[];
  title: string;
  type: ExerciseType;
  unit?: Unit;
  user?: string;
}

export interface ExerciseDefinitionModel
  extends Model<ExerciseDefinitionDocument> {
  addNewSession: (exercise: {
    definitionId: string;
    sets: ISet[];
    timeTaken: string;
  }) => ExerciseDocument;
  getUnit: (id: string) => Unit;
  // FIXME: occluded by Model.update
  update: (updatedExercise: ExerciseDefinitionDocument) => any;
  removeHistorySession: (
    definitionId: string,
    exerciseId: string
  ) => ExerciseDefinitionDocument;
}

export interface UserDocument extends Document {
  email: string;
  exercises: ExerciseDefinitionDocument[];
  password: string;
  firstName: string;
  id: string;
  lastName: string;
}

export interface UserModel extends Model<UserDocument> {
  login: (details: { password: string; email: string }) => UserDocument;
  register: (details: {
    firstName: string;
    lastName: string;
    password: string;
    email: string;
  }) => UserDocument;
  createExercise: (
    exercise: ExerciseDefinitionDocument
  ) => ExerciseDefinitionDocument;
}
