import { parseJSON } from './services';

let abortController = new AbortController()

export const api = async(url, method = "get", body, abortPrevious) => {
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
				const contentType = resp.headers.get('content-type');

				// If content-type is application/json, parse response. Invalid JSON will throw error, causing promise to reject.
				if(contentType && contentType.search('application/json') !== -1) {
					return JSON.parse(responseText);
				}

				// If content-type is not application/json, try to parse response. If parse fails, just return response text.
				try {
					const data = JSON.parse(responseText);
					console.warn(`'${url}' returned JSON with incorrect content-type header of '${contentType}'`);
					return data;
				} catch(e) {
					return responseText;
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

