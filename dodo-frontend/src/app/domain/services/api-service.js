import { tryToParseJSON } from './services';

export const api = async(url, method = "get", body, abortController) => {
	return fetch(url, {
		method,
		body: JSON.stringify(body),
		headers: {
			"Content-Type": "application/json",
			Accept: "application/json"
		},
		credentials: "include",
		signal: abortController ? abortController.signal : undefined,
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
				response: tryToParseJSON(responseText),
				status: resp.status,
				message: resp.statusText
			}
		})
	})
	.catch(resp => {
		if(resp instanceof Error) {
			if(resp.name !== 'AbortError') {
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
				status: -1,
				message: 'Network error'
			}
		}
		throw resp;
	})
}

