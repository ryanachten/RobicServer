const mongoose = require("mongoose");

mongoose.plugin(schema => {
  schema.options.usePushEach = true;
});

require("./sessionDefinition");

// TODO: delete following imports
require("./song");
require("./lyric");
