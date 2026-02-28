import { User } from "../models/user.model.js";
import { hashPassword, comparePasswords } from "../utils/password.util.js";
import { generateToken, generateRefreshToken } from "../services/token.service.js";
import jwt from "jsonwebtoken";
import cookieParser from "cookie-parser";

const options ={
    httpOnly: true,
    secure: true
}

const registerUser = async (req,res) => {
    const { username, email, fullName, role, password } = req.body;

    if(!username || !email || !fullName || !password) {
        return res.status(400).json({ error: "All fields are required" });
    }

    const preRegisterUser = await User.findOne({ $or: [ { username }, { email } ] });

    const roleEnum = role.toLowerCase() === "admin" ? "ADMIN" : "USER";

    if(preRegisterUser) {
        return res.status(401).json({ error: "User already exists" });
    }

    const hashedPassword = await hashPassword(password);

    const user = await User.create({
        username,
        email,
        fullName,
        role: roleEnum,
        password: hashedPassword
    });

    if(!user){
        return res.status(500).json({ error: "Something went wrong user not created" })   
    }

    return res.status(201).json(user);
}

const loginUser = async (req,res) => {
    console.log({reqBody: req.body});
    const {username, email, password} = req.body;
    console.log({username, email, password});

    if((!username && !password) || (!email && !password)){
        console.error("All fields are required");
        throw error;
    }

    const user = await User.findOne({ $or: [{ username }, { email }] });

    if (!user) {
        const err = new Error("User not found");
        err.statusCode = 404;
        throw err;
    }

    const isPasswordValid = await comparePasswords(password, user.password);

    if (!isPasswordValid) {
        const err = new Error("Invalid credentials");
        err.statusCode = 401;
        throw err;
    }

    const accessToken = generateToken(user);
    const refreshToken = generateRefreshToken(user);

    user.refreshToken = refreshToken;
    await user.save();

    return res.status(200)
    .cookie("accessToken", accessToken, options)
    .cookie("refreshToken", refreshToken, options)
    .json({ accessToken, refreshToken });
}

const logoutUser = async (req, res) => {
    const accessToken = req.cookies.accessToken;

    if (!accessToken) {
        return res.status(401).json({ error: "Unauthorized" });
    }
    let decodedToken;
    try{
         decodedToken = jwt.verify(accessToken, process.env.JWT_SECRET);
    } catch (error) {
        console.log("Invalid token, token expired");
        res.clearCookie("accessToken", options);
        res.clearCookie("refreshToken", options);
        return res.status(401).json({ error: "Unauthorized" });
    }

    const user = await User.findById(decodedToken.id);

    await User.findOneAndUpdate(
        { _id: user._id }, 
        { 
            $unset: { refreshToken: true }
        },
        {
            new: true
        }
    );

    return res.status(200)
    .clearCookie("accessToken", options)
    .clearCookie("refreshToken", options)
    .json({ message: "Logged out successfully" });
}


const refreshAccessToken = async () => {
    const refreshToken = req.cookies.refreshToken;

    if (!refreshToken) {
        return res.status(401).json({ error: "Unauthorized" });
    }

    const decodedToken = jwt.verify(refreshToken, process.env.REFRESH_TOKEN_SECRET);

    const user = await User.findById(decodedToken.id);

    const accessToken = generateToken(user);

    return res.status(200)
    .cookie("accessToken", accessToken, options)
    .json({ accessToken });
}


export { registerUser, loginUser, logoutUser, refreshAccessToken };