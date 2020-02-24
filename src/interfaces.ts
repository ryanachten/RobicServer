import { Document } from 'mongoose';

export type IRequest = {
  headers: {
    authorization: string;
  };
  user: IUser;
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
  definition: IExerciseDefinition;
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

export interface IExerciseDefinition extends Document {
  childExercises?: IExerciseDefinition[];
  history: IExercise[];
  id: string;
  primaryMuscleGroup: MuscleGroup[];
  title: string;
  type: ExerciseType;
  unit?: IUnit;
  user: IUser;
}

export interface IUser extends Document {
  email: string;
  exercises: IExerciseDefinition[];
  password: string;
  firstName: string;
  id: string;
  lastName: string;
}
