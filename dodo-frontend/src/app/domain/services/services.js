export const tryToParseJSON = (input) => {
	try {
		return JSON.parse(input);
	} catch (e) {
		return input;
	}
};

const REDIRECT_URL_PARAM = "returnurl";
const PLACEHOLDER_BASE_URL = "http://example.com";
// example.com used here because URL() requires a base not just a
// relative path. It is just a placeholder, however, and affects nothing.

export const addReturnPathToRoute = (route, returnPath) => {
	const url = new URL(route, PLACEHOLDER_BASE_URL);
	url.searchParams.set(REDIRECT_URL_PARAM, returnPath);
	return url.pathname + url.search;
};

export const getReturnPath = (location) => {
	const search = new URLSearchParams(location.search);
	const param = [...search.entries()].find(
		(entry) => entry[0].toLowerCase() === REDIRECT_URL_PARAM.toLowerCase()
	);
	return param?.[1];
};

export const keepReturnPathParam = (newRoute, location) => {
	const returnURL = getReturnPath(location);
	return returnURL ? addReturnPathToRoute(newRoute, returnURL) : newRoute;
};

export const passwordContainsNoSymbol = (password) =>
	!/^(?=.*[@#$%^&+=!]).*$/.test(password);
export const emailIsValid = (email) => /\w+@\w+\.\w{2,}/.test(email);
export const strNotEmptyAndLengthBelow = (minLength, str) =>
	!!str && str.length < minLength;

export const WARNING__BLOCK_JS_THREAD_FOR_MS = (milliseconds) => {
	const start = new Date().getTime();
	for (let i = 0; i < 1e7; i++)
		if (new Date().getTime() - start > milliseconds) {
			break;
		}
};
