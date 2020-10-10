let abortController = new AbortController()

// The auth URLs are a bit odd. They return the HTML for the login page if you are not logged in, or JSON if you are.
// That is why we have this special service for them.

export const auth = async(url, method = "get", body, abortSignal) => {
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
		return resp.text()
		.then(responseText => {
			if(resp.ok) {
				try {
					return JSON.parse(responseText);
				} catch(e) {
					throw new Error('User is not logged in');
				}
			}
			throw new Error('Server error');
		})
	})
	.catch(resp => {
		throw new Error('Network error');
	})
}

