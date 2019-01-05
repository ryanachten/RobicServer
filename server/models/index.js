const mongoose = require("mongoose");

mongoose.plugin(schema => {
  schema.options.usePushEach = true;
});

require("./user");
require("./sessionDefinition");
require("./session");
require("./exerciseDefinition");

// TODO: delete following imports
require("./song");
require("./lyric");
