const mongoose = require("mongoose");
const Schema = mongoose.Schema;

const ExerciseSchema = new Schema({
  definition: {
    type: Schema.Types.ObjectId,
    ref: "exerciseDefinition"
  },
  date: { type: Date },
  sets: [
    {
      reps: { type: Number, default: 0 },
      value: { type: Number, default: 0 }
    }
  ],
  timeTaken: { type: String },
  netValue: { type: Number, default: 0 }
});

ExerciseSchema.statics.getDefinition = function(id) {
  return this.findById(id)
    .populate("definition")
    .then(exercise => exercise.definition);
};

ExerciseSchema.statics.update = async function(exerciseId, sets, timeTaken) {
  const exercise = await this.findById(exerciseId);
  exercise.sets = sets;
  exercise.timeTaken = timeTaken;
  return exercise.save();
};

mongoose.model("exercise", ExerciseSchema);
