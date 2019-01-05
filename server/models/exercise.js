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
  netValue: { type: Number, default: 0 }
});

ExerciseSchema.statics.addSet = async ({ id, reps, value }) => {
  const exercise = await this.findById(id);
  exercise.sets.push({
    reps,
    value
  });
  await exercise.save();
  return exercise;
};

ExerciseSchema.statics.updateNetValue = async (id, netValue) => {
  const exercise = await this.findById(id);
  exercise.netValue = netValue;
  await exercise.save();
  return exercise;
};

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

mongoose.model("exercise", ExerciseSchema);
