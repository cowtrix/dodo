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
	return search.get(REDIRECT_URL_PARAM);
};

export const keepReturnPathParam = (newRoute, location) => {
	const returnURL = getReturnPath(location);
	return returnURL ? addReturnPathToRoute(newRoute, returnURL) : newRoute;
};
