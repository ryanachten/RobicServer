export enum MuscleGroup {
  CHEST = "Chest",
  FOREARMS = "Forearms",
  LATS = "Lats",
  // MIDDLE_BACK = "Middle Back",
  LOWER_BACK = "Lower Back",
  NECK = "Neck",
  HAMS = "Hamstrings",
  QUADS = "Quadriceps",
  CALVES = "Calves",
  TRICEPS = "Triceps",
  TRAPS = "Traps",
  SHOULDERS = "Shoulders",
  ABS = "Abdominals",
  OBLIQUES = "Obliques",
  GLUTES = "Glutes",
  BICEPS = "Biceps"
  // ADDUCTORS = "Adductors",
  // ABDUCTORS = "Abductors"
}

export enum Unit {
  kg = "kg",
  min = "min",
  bodyweight = "body weight"
}

export interface SetExercise {
  id: string;
  reps: number;
  value: number;
  unit: Unit;
}

export type Set = {
  exercises?: SetExercise[];
  reps: number; //TODO: should be optional
  value: number; //TODO: should be optional
};

export enum ExerciseType {
  CIRCUIT = "Circuit",
  STANDARD = "Standard",
  SUPERSET = "Superset"
}

export type Exercise = {
  id: string;
  date: string;
  definiton: IExerciseDefinition;
  sets: Set[];
  timeTaken: string;
};

export type PersonalBest = {
  netValue: { value: number };
  setCount: { value: number };
  timeTaken: { value: number };
  totalReps: { value: number };
  value: { value: number };
};

export interface IExerciseDefinition extends Document {
  childExercises?: IExerciseDefinition[];
  history: Exercise[];
  id: string;
  primaryMuscleGroup: MuscleGroup[];
  title: string;
  type: ExerciseType;
  unit?: Unit;
}

export type User = {
  email: string;
  firstName: string;
  id: string;
  lastName: string;
};
