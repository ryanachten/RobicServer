const mongoose = require("mongoose");
const Schema = mongoose.Schema;

const SessionSchema = new Schema({
  title: { type: String },
  date: { type: Date, default: Date.now },
  exercises: {
    type: Schema.Types.ObjectId,
    ref: "exercise"
  },
  sessions: [
    {
      type: Schema.Types.ObjectId,
      ref: "session"
    }
  ]
});

SessionSchema.statics.addExercise = function(id, content) {
  const Exercise = mongoose.model("exercise");

  return this.findById(id).then(session => {
    // FIXME: not sure this is really what I want,
    //  - will return entire Exercise object?
    const exercise = new Exercise({ content, session });
    exercise.sessions.push(session);
    return Promise.all([exercise.save(), session.save()]).then(
      ([exercise, session]) => session
    );
  });
};

SessionSchema.statics.findExercises = function(id) {
  return this.findById(id)
    .populate("exercises")
    .then(session => session.exercises);
};

mongoose.model("session", SessionSchema);
