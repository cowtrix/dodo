export const api = (url, method = 'get', body) =>
	fetch(url, {
		method,
		body: JSON.stringify(body),
		headers: {
			'Content-Type': 'application/json',
			'Accept': 'application/json',
		},
	})
		.then((resp) => resp.json())