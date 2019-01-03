const mongoose = require("mongoose");
const validator = require("validator");
const Schema = mongoose.Schema;

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

mongoose.model("user", UserSchema);
