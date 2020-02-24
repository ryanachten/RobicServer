import mongoose = require('mongoose');

mongoose.plugin((schema: any) => {
  schema.options.usePushEach = true;
});

require('./user');
require('./exercise');
require('./exerciseDefinition');
