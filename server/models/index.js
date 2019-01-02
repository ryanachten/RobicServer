const mongoose = require("mongoose");

mongoose.plugin(schema => {
  schema.options.usePushEach = true;
});

require("./sessionDefinition");
require("./session");

// TODO: delete following imports
require("./song");
require("./lyric");
