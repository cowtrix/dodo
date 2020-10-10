export const parseJSON = input => {
    try {
        return JSON.parse(input);
    } catch(e) {
        return input;
    }
}