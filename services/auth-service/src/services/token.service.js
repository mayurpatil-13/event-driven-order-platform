import jwt from 'jsonwebtoken';

const generateToken = (user) => {
    return jwt.sign(
        {id: user._id, fullName: user.fullName, role: user.role},
        process.env.JWT_SECRET,
        {expiresIn: '1h'}
    )
}

const generateRefreshToken = (user) => {
    return jwt.sign(
        {id: user.id},
        process.env.REFRESH_TOKEN_SECRET,
        {expiresIn: '8h'}
    )
}

export { generateToken, generateRefreshToken}
