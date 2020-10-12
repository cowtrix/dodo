import { parseJSON } from './services';

let abortController = new AbortController()

// The auth URLs are a bit odd. They return the HTML for the login page if you are not logged in, or JSON if you are.
// That is why we have this special service for them.

export const auth = async(url, method = "get", body, abortPrevious) => {
	abortController.abort()
	abortController = new AbortController()

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
						response: parseJSON(responseText),
						status: resp.status,
						message: 'User is not logged in'
					}
				}
			}
			// eslint-disable-next-line no-throw-literal
			throw {
				response: parseJSON(responseText),
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

