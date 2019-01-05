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
  history: [
    {
      type: Schema.Types.ObjectId,
      ref: "session"
    }
  ]
});

ExerciseDefinitionSchema.statics.addNewSession = function(definitionId) {
  const Exercise = mongoose.model("exercise");

  return this.findById(definitionId).then(exerciseDef => {
    const activeExercise = new Exercise({
      date: Date.now(),
      definition: exerciseDef
    });
    // Add the active session to the history log
    exerciseDef.history.push(activeExercise);
    // Save both the updated definition and new active session, return new session
    return Promise.all([exerciseDef.save(), activeExercise.save()]).then(
      ([exerciseDef, activeExercise]) => activeExercise
    );
  });
};

ExerciseDefinitionSchema.statics.getHistory = function(id) {
  return (
    this.findById(id)
      // Find the definition and then return the history log
      .populate("history")
      .then(exercise => exercise.history)
  );
};

mongoose.model("exerciseDefinition", ExerciseDefinitionSchema);
