import { LOGIN } from '../urls';

let abortController = new AbortController()

export const login = async (url = LOGIN, method = 'post', body) => {
	abortController.abort()
	abortController = new AbortController()

	return fetch(url, {
		method,
		body,
		headers: {
			"Content-Type": "application/json",
			Accept: "application/json"
		},
		credentials: "include",
		signal: abortController.signal
	})
	.then(resp => {
		return resp.text()
		.then(responseText => {
			if(resp.ok) {
				try {
					return JSON.parse(responseText);
				} catch(e) {
					// eslint-disable-next-line no-throw-literal
					throw 'Server error'; // This should never happen
				}
			}
			// eslint-disable-next-line no-throw-literal
			throw 'Unknown username or password';
		})
	})
}
// TODO: This could be refactored to use api-action if we change the way we handle error messages...
