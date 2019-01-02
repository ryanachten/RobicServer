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
  history: [
    {
      type: Schema.Types.ObjectId,
      ref: "session"
    }
  ]
});

SessionDefinitionSchema.statics.addNewSession = function(definitionId) {
  const Session = mongoose.model("session");

  return this.findById(definitionId).then(sessionDef => {
    const activeSession = new Session({
      date: Date.now(),
      definition: sessionDef
      // TODO: add exercises based of latest history object
    });
    // Add the active session to the history log
    sessionDef.history.push(activeSession);
    // Save both the updated definition and new active session, return new session
    return Promise.all([sessionDef.save(), activeSession.save()]).then(
      ([sessionDef, activeSession]) => activeSession
    );
  });
};

SessionDefinitionSchema.statics.getHistory = function(id) {
  return (
    this.findById(id)
      // Find the definition and then return the history log
      .populate("history")
      .then(session => session.history)
  );
};

mongoose.model("sessionDefinition", SessionDefinitionSchema);
