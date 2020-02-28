import {
  ExerciseDefinitionDocument,
  ExerciseDefinitionModel,
  ISet,
  ExerciseDocument,
  Unit
} from '../interfaces';

import mongoose = require('mongoose');

const { Schema } = mongoose;
const Exercise = mongoose.model('exercise');

/*
  Houses history and aggregate data for the Exercise schema
*/

const ExerciseDefinitionSchema = new Schema({
  user: {
    type: Schema.Types.ObjectId,
    ref: 'user'
  },
  title: { type: String },
  unit: { type: String },
  primaryMuscleGroup: [{ type: String }],
  type: { type: String },
  childExercises: [
    {
      type: Schema.Types.ObjectId,
      ref: 'exerciseDefinition'
    }
  ],
  personalBest: {
    value: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: 'exercise'
      }
    },
    setCount: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: 'exercise'
      }
    },
    totalReps: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: 'exercise'
      }
    },
    netValue: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: 'exercise'
      }
    },
    timeTaken: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: 'exercise'
      }
    }
  },
  history: [
    {
      type: Schema.Types.ObjectId,
      ref: 'exercise'
    }
  ]
});

ExerciseDefinitionSchema.statics.addNewSession = async ({
  definitionId,
  sets,
  timeTaken
}: {
  definitionId: string;
  sets: ISet;
  timeTaken: string;
}): Promise<ExerciseDocument> => {
  const exerciseDef = (await ExerciseDefinitionSchema.methods.findById(
    definitionId
  )) as ExerciseDefinitionDocument;
  const activeExercise = new Exercise({
    date: Date.now(),
    definition: definitionId,
    sets,
    timeTaken,
    netValue: 0
  }) as ExerciseDocument;
  // Add the active session to the history log
  exerciseDef.history.push(activeExercise);
  // Save both the updated definition and new active exercise, return new exercise
  return Promise.all([exerciseDef.save(), activeExercise.save()]).then(
    ([, newExercise]) => newExercise as ExerciseDocument
  );
};

ExerciseDefinitionSchema.statics.getChildExercises = function(
  id: string
): ExerciseDefinitionDocument[] {
  return this.findById(id)
    .populate('childExercises')
    .then((exercise: ExerciseDefinitionDocument) => exercise.childExercises);
};

ExerciseDefinitionSchema.statics.getHistory = function(
  id: string
): ExerciseDocument[] {
  return (
    this.findById(id)
      // Find the definition and then return the history log
      .populate('history')
      .then((exercise: ExerciseDefinitionDocument) => exercise.history)
  );
};

ExerciseDefinitionSchema.statics.getUnit = function(id: string): Unit {
  return this.findById(id).then(
    (exercise: ExerciseDefinitionDocument) => exercise.unit
  );
};

ExerciseDefinitionSchema.statics.update = async function({
  id,
  title,
  unit,
  primaryMuscleGroup,
  type,
  childExercises
}: ExerciseDefinitionDocument): Promise<ExerciseDefinitionDocument> {
  const definition = await this.findById(id);
  definition.title = title;
  definition.unit = unit;
  definition.primaryMuscleGroup = primaryMuscleGroup;
  definition.type = type;
  definition.childExercises = childExercises;
  return definition.save();
};

ExerciseDefinitionSchema.statics.removeHistorySession = async function(
  definitionId: string,
  exerciseId: string
): Promise<ExerciseDefinitionDocument> {
  // Remove the exercise from definition history
  const definition = await this.findById(definitionId);

  // Delete the exercise from the database
  // this appears to handle removing the defintion history too
  await Exercise.findById(exerciseId)
    .remove()
    .exec();

  return definition;
};

mongoose.model<ExerciseDefinitionDocument, ExerciseDefinitionModel>(
  'exerciseDefinition',
  ExerciseDefinitionSchema
);
