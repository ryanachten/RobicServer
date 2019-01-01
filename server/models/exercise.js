const mongoose = require("mongoose");
const Schema = mongoose.Schema;

const ExerciseSchema = new Schema({
  title: { type: String },
  unit: { type: String },
  lastActive: { type: Date, default: Date.now },
  user: {
    type: Schema.Types.ObjectId,
    ref: "user"
  },
  sessions: [
    {
      type: Schema.Types.ObjectId,
      ref: "session"
    }
  ]
});

ExerciseSchema.statics.addSession = function(id, content) {
  const Session = mongoose.model("session");

  return this.findById(id).then(exercise => {
    // FIXME: not sure this is really what I want,
    //  - will return entire Session object?
    const session = new Session({ content, exercise });
    exercise.sessions.push(session);
    return Promise.all([session.save(), exercise.save()]).then(
      ([session, exercise]) => exercise
    );
  });
};

ExerciseSchema.statics.findSessions = function(id) {
  return this.findById(id)
    .populate("sessions")
    .then(exercise => exercise.sessions);
};

mongoose.model("exercise", ExerciseSchema);
