const mongoose = require("mongoose");
const Schema = mongoose.Schema;

const SessionSchema = new Schema({
  definition: {
    type: Schema.Types.ObjectId,
    ref: "sessionDefinition"
  },
  date: { type: Date, default: Date.now },
  exercises: [
    {
      type: Schema.Types.ObjectId,
      ref: "exercise"
    }
  ]
});

SessionSchema.statics.addExercise = function(id, definitionId) {
  const Exercise = mongoose.model("exercise").findById(exerciseId);

  // Create empty Exercise
  return this.findById(id).then(session => {
    const exercise = new Exercise({
      definition: definitionId,
      session: session.id,
      sets: [],
      netValue: 0.0,
      weightChange: {
        delta: null,
        sign: ""
      }
    });
    session.exercises.push(exercise);
    return Promise.all([session.save(), exercise.save()]).then(
      ([session, exercise]) => exercise
    );
  });
};

SessionSchema.statics.getDefinition = function(id) {
  return this.findById(id)
    .populate("definition")
    .then(session => session.definition);
};

SessionSchema.statics.getExercises = function(id) {
  return this.findById(id)
    .populate("exercises")
    .then(session => session.exercises);
};

mongoose.model("session", SessionSchema);
