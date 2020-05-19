export const api = (url, method = "get", body) =>
	fetch(url, {
		method,
		body: JSON.stringify(body),
		headers: {
			"Content-Type": "application/json",
			Accept: "application/json"
		},
		credentials: "include"
	}).then(resp => resp.json())
