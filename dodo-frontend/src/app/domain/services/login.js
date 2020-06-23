import { LOGIN } from "app/domain/urls"

export const postLogin = async (username, password, rememberMe) =>
	fetch(LOGIN, {
		method: "post",
		body: JSON.stringify({
			username: username,
			password: password,
			rememberMe: rememberMe,
			redirect: ""
		}),
		headers: {
			"Content-Type": "application/json",
			Accept: "application/json"
		},
		credentials: "include"
	}).then(resp => resp.json())
// TODO: login doesn't return JSON (just a HTTP status code) so need to refactor if to use api service
