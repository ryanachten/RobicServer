const mongoose = require("mongoose");
const Schema = mongoose.Schema;

const ExerciseSchema = new Schema({
  definition: {
    type: Schema.Types.ObjectId,
    ref: "exerciseDefinition"
  },
  session: {
    type: Schema.Types.ObjectId,
    ref: "session"
  },
  sets: [
    {
      reps: { type: Number, default: 0 },
      value: { type: Number, default: 0 }
    }
  ],
  timeTaken: { type: Number, default: 0 },
  netValue: { type: Number, default: 0 }
});

ExerciseSchema.statics.getDefinition = function(id) {
  return this.findById(id)
    .populate("definition")
    .then(exercise => exercise.definition);
};

ExerciseSchema.statics.getSession = function(id) {
  return this.findById(id)
    .populate("session")
    .then(exercise => exercise.session);
};

ExerciseSchema.statics.update = async function(exerciseId, sets, timeTaken) {
  const exercise = await this.findById(exerciseId);
  exercise.sets = sets;
  exercise.timeTaken = timeTaken;
  return exercise.save();
};

mongoose.model("exercise", ExerciseSchema);
