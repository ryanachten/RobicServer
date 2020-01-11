const mongoose = require("mongoose");

mongoose.plugin(schema => {
  schema.options.usePushEach = true;
});

require("./user");
require("./exercise");
require("./exerciseDefinition");
