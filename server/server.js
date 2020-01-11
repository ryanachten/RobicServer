const bodyParser = require("body-parser");
const cors = require("cors");
const express = require("express");
const expressGraphQL = require("express-graphql");
const jwt = require("jsonwebtoken");
const mongoose = require("mongoose");

const models = require("./models");
const schema = require("./schema/schema");

require("dotenv").config();

const app = express();

/*** MongoLab configuration ***/
const MONGO_URI = process.env.MONGOLAB_URI;
if (!MONGO_URI) {
  throw new Error("You must provide a MongoLab URI");
}

mongoose.Promise = global.Promise;
mongoose.connect(MONGO_URI, {
  useCreateIndex: true,
  useNewUrlParser: true,
  useUnifiedTopology: true
});
mongoose.connection
  .once("open", () => console.log("Connected to MongoLab instance."))
  .on("error", error => console.log("Error connecting to MongoLab:", error));

/*** General middleware ***/
app.use(bodyParser.json());
app.use(cors());

/*** JWT verification middleware ***/
const JWT_PASSWORD_SECRET = process.env.JWT_PASSWORD_SECRET;
const verifyJwtToken = async req => {
  // Access token off client request header
  const token = req.headers.authorization;
  try {
    // Verify user using JWT secret
    const { user } = await jwt.verify(token, JWT_PASSWORD_SECRET);
    // Add the user to the request for parsing in the GQL middleware
    req.user = user;
  } catch (err) {
    console.log(err);
  }
  req.next();
};
app.use(verifyJwtToken);

/*** GraphQL middleware ***/
app.use(
  "/graphql",
  expressGraphQL(req => ({
    schema,
    graphiql: true,
    context: {
      // Add authed user object to GQL context
      user: req.user
    }
  }))
);

module.exports = app;
