const app = require("./src/server");

const port = process.env.PORT || 4000;

app.listen(port, () => {
  console.log("Listening on port", port);
});
