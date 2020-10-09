let abortController = new AbortController()

export const api = async(url, method = "get", body, abortSignal) => {
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
		signal: abortSignal ? abortController.signal : null,
	})
	.then(resp => {
		if(resp.ok) {
			return resp.text()
			.then(responseText => {
				const contentType = resp.headers.get('content-type');
				if(contentType && contentType.search('application/json') !== -1) {
					return JSON.parse(responseText);
				}
				return responseText;
			})
		}
		throw resp;
	})
}

