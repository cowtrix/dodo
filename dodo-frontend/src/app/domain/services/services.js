export const tryToParseJSON = input => {
    try {
        return JSON.parse(input);
    } catch(e) {
        return input;
    }
}