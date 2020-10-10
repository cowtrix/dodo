import { api, auth, login } from "../services"
import { REQUEST, FAILURE, SUCCESS } from "../constants"

import { LOGIN } from '../user/action-types';
import { LOGIN as LOGIN_URL } from '../urls';

const apiActionHandler = async (service, dispatch, action, url, cb, abortSignal, method, body) => {
	dispatch({
		type: action + REQUEST,
		payload: {
			action,
			url
		}
	})
	return service(url, method, body, abortSignal)
		.then(response => {
			if (response.status) {
				dispatch({
					type: action + FAILURE,
					payload: response
				})
			}
			else {
				dispatch({
					type: action + SUCCESS,
					payload: response
				})
			}
			if (cb) cb(response)
		})
		.catch(error => {
			console.log(error)
			if (cb) cb(false)
			dispatch({
				type: action + FAILURE,
				payload: error
			})
		})
}

export const apiAction = (dispatch, action, url, cb, abortSignal, method, body) => {
	return apiActionHandler(api, dispatch, action, url, cb, abortSignal, method, body);
}

export const authAction = (dispatch, action, url, cb, abortSignal, method, body) => {
	return apiActionHandler(auth, dispatch, action, url, cb, abortSignal, method, body);
}

export const loginAction = (dispatch, username, password, rememberMe) => {
	const body = JSON.stringify({
		username: username,
		password: password,
		rememberMe: rememberMe,
		redirect: ""
	});
	return apiActionHandler(login, dispatch, LOGIN, LOGIN_URL, undefined, undefined, 'post', body);
}