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

export enum IUnit {
  kg = 'kg',
  min = 'min',
  bodyweight = 'body weight'
}

export interface SetExercise {
  id: string;
  reps: number;
  value: number;
  unit: IUnit;
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

export interface IExercise extends Document {
  id: string;
  date: string;
  definition: ExerciseDefinitionDocument;
  sets: ISet[];
  timeTaken: string;
}

export type PersonalBest = {
  netValue: { value: number };
  setCount: { value: number };
  timeTaken: { value: number };
  totalReps: { value: number };
  value: { value: number };
};

export interface ExerciseDefinitionDocument extends Document {
  childExercises?: ExerciseDefinitionDocument[];
  history: IExercise[];
  id: string;
  primaryMuscleGroup: MuscleGroup[];
  title: string;
  type: ExerciseType;
  unit?: IUnit;
  user?: string;
}

export interface ExerciseDefinitionModel
  extends Model<ExerciseDefinitionDocument> {
  addNewSession: (exercise: {
    definitionId: string;
    sets: ISet[];
    timeTaken: string;
  }) => IExercise;
  update: (updatedExercise: ExerciseDefinitionDocument) => any; // FIXME: should be ExerciseDefinitionDocument but wont work
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
