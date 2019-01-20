const mongoose = require("mongoose");
const Schema = mongoose.Schema;

/*
  Houses history and aggregate data for the Session schema
*/

const SessionDefinitionSchema = new Schema({
  user: {
    type: Schema.Types.ObjectId,
    ref: "user"
  },
  title: { type: String },
  exercises: [
    {
      type: Schema.Types.ObjectId,
      ref: "exerciseDefinition"
    }
  ],
  history: [
    {
      type: Schema.Types.ObjectId,
      ref: "session"
    }
  ]
});

SessionDefinitionSchema.statics.addNewSession = async function(definitionId) {
  const Session = mongoose.model("session");
  const Exercise = mongoose.model("exercise");
  const ExerciseDefinition = mongoose.model("exerciseDefinition");

  const sessionDefinition = await this.findById(definitionId);
  const activeSession = new Session({
    date: Date.now(),
    definition: sessionDefinition
  });
  const exceriseDefinitions = sessionDefinition.exercises;
  const exercises = await Promise.all(
    exceriseDefinitions.map(async exerciseDefId => {
      const exerciseDef = await ExerciseDefinition.findById(exerciseDefId);
      const exercise = new Exercise({
        definition: exerciseDef,
        session: activeSession,
        sets: [],
        netValue: 0
      });
      // Add exercise to session
      activeSession.exercises.push(exercise);
      // Add exercise to definition
      exerciseDef.history.push(exercise);
      await exercise.save();
      await exerciseDef.save();
    })
  );
  // Add the active session to the history log
  sessionDefinition.history.push(activeSession);
  // Save both the updated definition and new active session, return new session
  await sessionDefinition.save();
  await activeSession.save();

  return activeSession;
};

SessionDefinitionSchema.statics.getHistory = function(id) {
  return (
    this.findById(id)
      // Find the definition and then return the history log
      .populate("history")
      .then(session => session.history)
  );
};

SessionDefinitionSchema.statics.getExercises = function(id) {
  return (
    this.findById(id)
      // Find the definition and then return the exercise list
      .populate("exercises")
      .then(session => session.exercises)
  );
};

mongoose.model("sessionDefinition", SessionDefinitionSchema);
