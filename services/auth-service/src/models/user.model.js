import {Schema, mongoose} from "mongoose";

const userSchema = new Schema({
    username:{
        type: String,
        required: true,
        unique: true,
        lowercase: true,
        trim: true
    },
    email:{
        type: String,
        required: true,
        unique: true,
        lowercase: true,
        trim: true
    },
    fullName:{
        type: String,
        required: true,
        lowercase: true,
        trim: true
    },
    role:{
        type:String,
        enum:['USER', 'ADMIN'],
        default:'USER'
    },
    password:{
        type: String,
        required: [true, "Password is required"],
        minlength: [6, "Password must be at least 6 characters long"]
    },
    refreshToken: {
        type: String
    }
},
{
    timestamps: true
});

export const User = mongoose.model("User", userSchema);
