import i18n from 'i18next';

let abortController = new AbortController()

export const api = async(url, method = "get", body, abortSignal) => {
	abortController.abort()
	abortController = new AbortController()

	return fetch(url, {
		method,
		body: JSON.stringify(body),
		headers: {
			"Content-Type": "application/json",
			Accept: "application/json",
			'Accept-Language': i18n.language
		},
		credentials: "include",
		signal: abortSignal ? abortController.signal : null
	}).then(resp => resp.json())
}

