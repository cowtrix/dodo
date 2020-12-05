import { tryToParseJSON } from './services';

const previousAbortControllers = {};

function handleAborting(abortController, url) {
	const urlPath = url.split(/[?#]/)[0]; // Get URL path without params or hashes  to use as request key
	if(previousAbortControllers[urlPath]) previousAbortControllers[urlPath].abort(); // Abort last request if there is one
	previousAbortControllers[urlPath] = abortController; // Sotre controller so we can cancel the next request
}

// The auth URLs are a bit odd. They return the HTML for the login page if you are not logged in, or JSON if you are.
// That is why we have this special service for them.

export const auth = async(url, method = "get", body, abortPrevious) => {
	const abortController = new AbortController();
	if (abortPrevious) handleAborting(abortController, url);

	return fetch(url, {
		method,
		body: JSON.stringify(body),
		headers: {
			"Content-Type": "application/json",
			Accept: "application/json"
		},
		credentials: "include",
		signal: abortPrevious ? abortController.signal : null,
	})
	.then(resp => {
		return resp.text()
		.then(responseText => {
			if(resp.ok) {
				try {
					return JSON.parse(responseText);
				} catch(e) {
					// eslint-disable-next-line no-throw-literal
					throw {
						response: tryToParseJSON(responseText),
						status: resp.status,
						message: 'User is not logged in'
					}
				}
			}
			// eslint-disable-next-line no-throw-literal
			throw {
				response: tryToParseJSON(responseText),
				status: resp.status,
				message: resp.statusText
			}
		})
	})
	.catch(resp => {
		if(resp instanceof Error) {
			if(resp.name === 'AbortError') {
				// eslint-disable-next-line no-throw-literal
				throw {
					response: undefined,
					status: 0,
					message: resp.message
				}
			}
			// eslint-disable-next-line no-throw-literal
			throw {
				response: undefined,
				status: 0,
				message: 'Network error'
			}
		}
		throw resp;
	})
}

