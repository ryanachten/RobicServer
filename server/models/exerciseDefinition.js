const mongoose = require("mongoose");
const Schema = mongoose.Schema;

/*
  Houses history and aggregate data for the Exercise schema
*/

const ExerciseDefinitionSchema = new Schema({
  user: {
    type: Schema.Types.ObjectId,
    ref: "user"
  },
  title: { type: String },
  unit: { type: String },
  personalBest: {
    value: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: "exercise"
      }
    },
    setCount: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: "exercise"
      }
    },
    totalReps: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: "exercise"
      }
    },
    netValue: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: "exercise"
      }
    },
    timeTaken: {
      value: { type: Number, default: 0 },
      exercise: {
        type: Schema.Types.ObjectId,
        ref: "exercise"
      }
    }
  },
  history: [
    {
      type: Schema.Types.ObjectId,
      ref: "exercise"
    }
  ]
});

ExerciseDefinitionSchema.statics.addNewSession = async function({
  definitionId,
  sets,
  timeTaken
}) {
  const Exercise = mongoose.model("exercise");
  const exerciseDef = await this.findById(definitionId);
  const activeExercise = new Exercise({
    date: Date.now(),
    definition: definitionId,
    sets,
    timeTaken,
    netValue: 0
  });
  // Add the active session to the history log
  exerciseDef.history.push(activeExercise);
  // Save both the updated definition and new active exercise, return new exercise
  return Promise.all([exerciseDef.save(), activeExercise.save()]).then(
    ([exerciseDef, activeExercise]) => activeExercise
  );
};

ExerciseDefinitionSchema.statics.getHistory = function(id) {
  return (
    this.findById(id)
      // Find the definition and then return the history log
      .populate("history")
      .then(exercise => exercise.history)
  );
};

ExerciseDefinitionSchema.statics.getPersonalBestExercise = async (
  id,
  record
) => {
  const definition = await this.findById(id);
  const exercise = await exercise.personalBest[record].populate("exercise");
  return exercise;
};

ExerciseDefinitionSchema.statics.update = async function(id, title, unit) {
  const definition = await this.findById(id);
  definition.title = title;
  definition.unit = unit;
  return definition.save();
};

mongoose.model("exerciseDefinition", ExerciseDefinitionSchema);
