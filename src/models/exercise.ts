import { IExercise, ISet } from '../interfaces';

import mongoose = require('mongoose');
const { Schema } = mongoose;

const ExerciseSchema = new Schema({
  definition: {
    type: Schema.Types.ObjectId,
    ref: 'exerciseDefinition',
  },
  date: { type: Date },
  sets: [
    {
      reps: { type: Number, default: 0 },
      value: { type: Number, default: 0 },
      exercises: [
        {
          id: {
            type: Schema.Types.ObjectId,
            ref: 'exerciseDefinition',
          },
          reps: { type: Number, default: 0 },
          value: { type: Number, default: 0 },
        },
      ],
    },
  ],
  timeTaken: { type: String },
  netValue: { type: Number, default: 0 },
});

ExerciseSchema.statics.getDefinition = function (id: string) {
  return this.findById(id)
    .populate('definition')
    .then((exercise: IExercise) => exercise.definition);
};

ExerciseSchema.statics.update = async function (
  exerciseId: string,
  sets: ISet[],
  timeTaken: string,
) {
  const exercise = await this.findById(exerciseId);
  exercise.sets = sets;
  exercise.timeTaken = timeTaken;
  return exercise.save();
};

mongoose.model<IExercise>('exercise', ExerciseSchema);
