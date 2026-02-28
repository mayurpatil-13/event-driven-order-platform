import bcrypt from 'bcrypt';

const hashPassword = async (password) => {
    return await bcrypt.hash(password, 10);
}

const comparePasswords = async (password, hashedPassword) => {
    return await bcrypt.compare(password, hashedPassword);
}

export { hashPassword, comparePasswords };
