const _ = require("lodash");
const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const mongoose = require("mongoose");
const validator = require("validator");
const Schema = mongoose.Schema;

// Extract secret from env for password salt
require("dotenv").config();
const JWT_PASSWORD_SECRET = process.env.JWT_PASSWORD_SECRET;

/*
  Houses user information and authentication details
*/

const UserSchema = new Schema({
  // emails must be unique and valid emails
  email: {
    type: String,
    required: true,
    trim: true,
    minlength: 1,
    unique: true,
    validate: {
      validator: value => validator.isEmail(value),
      message: "{VALUE} is not a valid email"
    }
  },
  password: { type: String, required: true, minlength: 6 },
  tokens: [
    {
      access: {
        type: String,
        required: true
      },
      token: {
        type: String,
        required: true
      }
    }
  ],
  firstName: { type: String },
  lastName: { type: String }
});

UserSchema.statics.register = async ({
  firstName,
  lastName,
  password,
  email
}) => {
  const User = mongoose.model("user");
  // Store user password using hashed password with 12 salt rounds
  const hashedPassword = await bcrypt.hash(password, 12);
  return await new User({
    firstName,
    lastName,
    email,
    password: hashedPassword
  }).save();
};

UserSchema.statics.login = async ({ password, email }) => {
  const User = mongoose.model("user");
  // Locate user by email address in DB.
  const locatedUser = await User.findOne({
    email
  });
  if (!locatedUser) {
    throw new Error(`No user with the email address ${email} was found`);
  }
  // Compared the provided email with the hashed version stored on the user object
  const validPassword = await bcrypt.compare(password, locatedUser.password);
  if (!validPassword) {
    throw new Error(`The password provided is incorrect`);
  }
  // Create signed JSON web token
  const token = jwt.sign(
    {
      // At this stage, only include user ID in token to minimise decodable information in token
      user: _.pick(locatedUser, ["id"])
    },
    JWT_PASSWORD_SECRET,
    {
      // Token will expire in one year
      expiresIn: "1y"
    }
  );
  return token;
};

mongoose.model("user", UserSchema);
