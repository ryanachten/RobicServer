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
      type: Date,
      ref: "session"
    }
  ]
});

ExerciseSchema.statics.addSession = function(id, content) {
  //  TODO: uncomment once session schema has been created
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
