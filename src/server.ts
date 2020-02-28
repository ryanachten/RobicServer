import { Request } from './interfaces';

import mongoose = require('mongoose');
import bodyParser = require('body-parser');
import cors = require('cors');
import express = require('express');
import expressGraphQL = require('express-graphql');
import jwt = require('jsonwebtoken');

require('./models');

import schema = require('./schema/schema');

require('dotenv').config();

const app = express();

/** * MongoLab configuration ** */
const MONGO_URI = process.env.MONGOLAB_URI;
if (!MONGO_URI) {
  throw new Error('You must provide a MongoLab URI');
}

// mongoose.Promise = global.Promise; // TODO: is this needed?
mongoose.connect(MONGO_URI, {
  useCreateIndex: true,
  useNewUrlParser: true,
  useUnifiedTopology: true
});
mongoose.connection
  .once('open', () => console.log('Connected to MongoLab instance.'))
  .on('error', (error: Error) =>
    console.log('Error connecting to MongoLab:', error)
  );

/** * General middleware ** */
app.use(bodyParser.json());
app.use(cors());

/** * JWT verification middleware ** */
const { JWT_PASSWORD_SECRET } = process.env;
const verifyJwtToken = async (req: Request): Promise<void> => {
  // Access token off client request header
  const token = req.headers.authorization;
  try {
    // Verify user using JWT secret
    const { user } = (await jwt.verify(token, JWT_PASSWORD_SECRET)) as Request;
    // Add the user to the request for parsing in the GQL middleware
    req.user = user;
  } catch (err) {
    console.log('Error getting user from JWT verification', err);
  }
  req.next();
};
app.use(verifyJwtToken);

/** * GraphQL middleware ** */
app.use(
  '/graphql',
  expressGraphQL((req: Request) => ({
    schema,
    graphiql: true,
    context: {
      // Add authed user object to GQL context
      user: req.user
    }
  }))
);

module.exports = app;
